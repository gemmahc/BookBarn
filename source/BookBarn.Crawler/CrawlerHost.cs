using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BookBarn.Crawler
{
    /// <summary>
    /// Hosts Crawler scheduling and execution.
    /// </summary>
    public class CrawlerHost : IObservable<HostInfo>
    {
        private ICrawlerFactory _crawlerFactory;
        private ICrawlerQueue _queue;
        private ILogger _logger;
        private CrawlerHostConfiguration _config;
        private ConcurrentDictionary<int, IObserver<HostInfo>> _observers;
        private List<Task<CrawlerResult>> _tasks;

        /// <summary>
        /// Track the endpoints visited to avoid cycles and duplicate work.
        /// </summary>
        private ConcurrentDictionary<Uri, Visit> _visits;

        /// <summary>
        /// Creates a new instance of the crawler host.
        /// </summary>
        /// <param name="config">The host configuration.</param>
        /// <param name="crawlerFactory">The factory used to generate instances of the requested crawlers.</param>
        /// <param name="queue">The queue the host uses for processing requests.</param>
        public CrawlerHost(CrawlerHostConfiguration config, ICrawlerFactory crawlerFactory, ICrawlerQueue queue, ILogger logger)
        {
            _config = config;
            _crawlerFactory = crawlerFactory;
            _queue = queue;
            _visits = new ConcurrentDictionary<Uri, Visit>();
            _tasks = new List<Task<CrawlerResult>>();
            _observers = new ConcurrentDictionary<int, IObserver<HostInfo>>();
            _logger = logger;
            Info = new HostInfo();
        }

        /// <summary>
        /// Gets the current host information. 
        /// </summary>
        public HostInfo Info { get; }

        /// <summary>
        /// Executes the crawler host.
        /// </summary>
        /// <param name="token">The cancellation token to stop the host.</param>
        /// <returns>Waitable task.</returns>
        public async Task RunAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Info.StartCycle();

                // Queue new crawlers in the request queue
                while (_tasks.Count < _config.MaxConcurrentCrawlers && await _queue.HasWork())
                {
                    var request = await _queue.GetNext();

                    if (request == null)
                    {
                        // ToDo: Log no work returned
                        break;
                    }

                    Visit visit = _visits.GetOrAdd(request.Endpoint, new Visit() { LastResult = Result.Pending, Visits = 0 });

                    if (visit.LastResult != Result.Pending)
                    {
                        _logger.LogInformation("Skipping duplicate endpoint [{endpoint}]", request.Endpoint);
                        Info.DuplicatesSkippedLastCycle.Add(request.Endpoint);
                        continue;
                    }

                    try
                    {
                        Crawler crawler = GetInstance(request);

                        // Set the visit to in progress and run the crawler.
                        _visits.AddOrUpdate(
                                    request.Endpoint,
                                    new Visit() { LastResult = Result.InProgress, Visits = 1 },
                                    (key, old) =>
                                    {
                                        old.LastResult = Result.InProgress;
                                        old.Visits += 1;
                                        return old;
                                    });

                        _logger.LogInformation("Running crawler [{type}] for endpoint [{endpoint}]", request.RequestedCrawler.Name, request.Endpoint);
                        var task = Task<CrawlerResult>.Run(async () => await crawler.RunAsync());
                        _tasks.Add(task);

                        Info.ScheduledLastCycle.Add(request.Endpoint);
                        Info.CurrentlyRunning.Add(request.Endpoint);
                    }
                    catch (UnknownCrawlerTypeException ex)
                    {
                        // log and skip the request.
                        _logger.LogError(ex, "Unknown crawler type [{type}] encountered. Skipping.", ex.Type.FullName);
                    }
                }

                // Check for completed crawlers and clear from task list.
                if (_tasks.Any())
                {
                    Task<CrawlerResult> resultTask = await Task<CrawlerResult>.WhenAny(_tasks);
                    await resultTask;
                    _tasks.Remove(resultTask);
                    HandleResult(resultTask.Result);
                }

                if (_tasks.Count == 0 && !await _queue.HasWork())
                {
                    _logger.LogInformation("No pending work. Sleeping.");
                    Info.HostIdle = true;
                    // No tasks pending in the queue and no tasks running. Sleep for a bit and check again.
                    await Task.Delay(TimeSpan.FromMilliseconds(_config.IdlePollingIntervalMilliseconds));
                }

                NotifyObservers();
            }

            _logger.LogInformation("Crawler host stopping.");
            ClearObservers();
        }


        /// <summary>
        /// Gets an instance of the requested crawler.
        /// </summary>
        /// <param name="request">The request information.</param>
        /// <returns>An instance of the requested crawler.</returns>
        /// <exception cref="InvalidProgramException">Thrown if the expected method information is not present in the crawler factory.</exception>
        /// <exception cref="UnknownCrawlerTypeException">Thrown if the requested Crawler is not of a known type.</exception>
        private Crawler GetInstance(CrawlRequest request)
        {
            MethodInfo? create = typeof(ICrawlerFactory).GetMethod(nameof(ICrawlerFactory.Create));

            if (create == null)
            {
                throw new InvalidProgramException($"Unexpected null method for Create<T> on ICrawlerFactory");
            }

            MethodInfo generic = create.MakeGenericMethod(request.RequestedCrawler);
            Crawler? crawler = generic.Invoke(_crawlerFactory, new[] { request.Endpoint }) as Crawler;

            if (crawler == null)
            {
                throw new UnknownCrawlerTypeException(request.RequestedCrawler);
            }

            return crawler;
        }

        /// <summary>
        /// Handles subsequent actions for the specified crawler result.
        /// </summary>
        /// <param name="result">The crawler result.</param>
        private void HandleResult(CrawlerResult result)
        {
            _logger.LogInformation(
                "Crawler [{type}] for endpoint [{endpoint}] completed with result [{result}]",
                result.CrawlerType.Name,
                result.Endpoint,
                result.Result
                );
            Info.CurrentlyRunning.Remove(result.Endpoint);
            Info.CompletedLastCycle++;

            if (result.Result == Result.Success)
            {
                Info.SucceededLastCycle.Add(result.Endpoint);
                _visits.AddOrUpdate(
                    result.Endpoint,
                    new Visit() { LastResult = Result.Success, Visits = 1 },
                    (key, old) =>
                    {
                        old.LastResult = Result.Success;
                        return old;
                    });

                // Queue any of the child crawlers that were dispatched by this crawler.
                
                foreach (var kvp in result.ToDispatch)
                {
                    CrawlRequest request = new CrawlRequest(kvp.Key, kvp.Value);
                    Info.ChildrenProcessedLastCycle.Add(request.Endpoint);
                    QueueRequest(request);
                }
            }
            else if (result.Result == Result.Failure)
            {
                Info.FailedLastCycle.Add(result.Endpoint);
                Visit visit = _visits.AddOrUpdate(
                                            result.Endpoint,
                                            new Visit() { LastResult = Result.Failure, Visits = 1 },
                                            (key, old) =>
                                            {
                                                old.LastResult = Result.Failure;
                                                return old;
                                            });

                if (visit.Visits <= _config.FailedCrawlerRetryCount)
                {
                    // Re-queue this crawler if retries haven't been exhausted.
                    CrawlRequest request = new CrawlRequest(result.Endpoint, result.CrawlerType);
                    QueueRequest(request);
                }
            }
            else
            {
                // ToDo: Log other results here. Should not occur.
            }
        }

        /// <summary>
        /// Queues a crawler request.
        /// </summary>
        /// <param name="request">The request.</param>
        private void QueueRequest(CrawlRequest request)
        {
            if (_visits.ContainsKey(request.Endpoint))
            {
                Visit visit = _visits[request.Endpoint];
                if (visit.LastResult == Result.Failure && visit.Visits <= _config.FailedCrawlerRetryCount)
                {
                    Info.RetriesQueuedLastCycle.Add(request.Endpoint);
                    // If we've seen the request on this endpoint previously, the only only condition 
                    // we will re-queue it is if it hasn't exhausted retries on failure

                    _logger.LogInformation("Endpoint [{endpoint}] previously failed {visits} times. Retrying.", request.Endpoint, visit.Visits);
                }
                else
                {
                    // Already visited this endpoint (either successfully or exhausted retries), don't queue it again.
                    Info.DuplicatesSkippedLastCycle.Add(request.Endpoint);
                    _logger.LogInformation("Skip adding duplicate endpoint [{endpoint}] to queue.", request.Endpoint);
                    return;
                }
            }

            // Set the visit to pending and add it to the process queue.
            _visits.AddOrUpdate(
                        request.Endpoint,
                        new Visit { LastResult = Result.Pending, Visits = 0 },
                        (key, old) =>
                        {
                            old.LastResult = Result.Pending;
                            return old;
                        });

            _logger.LogInformation("Adding crawler [{type}] for endpoint [{endpoint}] to the queue.", request.RequestedCrawler.Name, request.Endpoint);
            _queue.Enqueue(request);
        }
        #region IObservable

        /// <summary>
        /// Subscribes to host events
        /// </summary>
        /// <param name="observer">The observer to deliver host information to.</param>
        /// <returns>The host information representing a snapshot of current execution.</returns>
        public IDisposable Subscribe(IObserver<HostInfo> observer)
        {
            int lastKey = 0;

            foreach (var kvp in _observers)
            {
                if (kvp.Value == observer)
                {
                    return new HostObserverUnsubscriber(_observers, kvp.Key);
                }

                lastKey = Math.Max(lastKey, kvp.Key);
            }

            lastKey++;

            while (!_observers.TryAdd(lastKey, observer))
            {
                lastKey++;
            }

            return new HostObserverUnsubscriber(_observers, lastKey);
        }

        /// <summary>
        /// Notify subscribed observers of the current host info.
        /// </summary>
        private void NotifyObservers()
        {
            foreach (var kvp in _observers)
            {
                kvp.Value.OnNext(Info.Clone());
            }
        }

        /// <summary>
        /// Clear subscriptions to the current host information.
        /// </summary>
        private void ClearObservers()
        {
            foreach (var kvp in _observers)
            {
                kvp.Value.OnCompleted();
            }
            _observers.Clear();
        }

        #endregion

    }
}
