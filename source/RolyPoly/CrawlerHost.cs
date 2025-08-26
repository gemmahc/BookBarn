using System.Collections.Concurrent;
using System.Reflection;

namespace RolyPoly
{
    /// <summary>
    /// Hosts Crawler scheduling and execution.
    /// </summary>
    public class CrawlerHost
    {
        private ICrawlerFactory _crawlerFactory;
        private ICrawlerQueue _queue;
        private const int MAX_CONCURRENT_CRAWLERS = 10;
        private const int IDLE_POLLING_INTERVAL_SECONDS = 10;
        private const int FAILED_CRAWLER_RETRY_COUNT = 3;

        /// <summary>
        /// Track the endpoints visited to avoid cycles and duplicate work.
        /// </summary>
        private ConcurrentDictionary<Uri, Visit> _visits;

        public CrawlerHost(ICrawlerFactory crawlerFactory, ICrawlerQueue queue)
        {
            _crawlerFactory = crawlerFactory;
            _queue = queue;
            _visits = new ConcurrentDictionary<Uri, Visit>();
        }

        /// <summary>
        /// Executes the crawler host.
        /// </summary>
        /// <param name="token">The cancellation token to stop the host.</param>
        /// <returns>Waitable task.</returns>
        public async Task Run(CancellationToken token)
        {
            List<Task<CrawlerResult>> tasks = new List<Task<CrawlerResult>>();

            while (!token.IsCancellationRequested)
            {
                // Queue new crawlers in the request queue
                while (tasks.Count < MAX_CONCURRENT_CRAWLERS || !await _queue.HasWork())
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
                        // ToDo: Log duplicate and continue
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
                        var task = Task<CrawlerResult>.Run(async () => await crawler.RunAsync());
                        tasks.Add(task);
                    }
                    catch (UnknownCrawlerTypeException ex)
                    {
                        // log and skip the request.
                    }
                }

                // Check for completed crawlers and clear from task list.
                Task<CrawlerResult> resultTask = await Task<CrawlerResult>.WhenAny(tasks);
                await resultTask;
                tasks.Remove(resultTask);
                HandleResult(resultTask.Result);

                if (tasks.Count == 0 && !await _queue.HasWork())
                {
                    // No tasks pending in the queue and no tasks running. Sleep for a bit and check again.
                    await Task.Delay(TimeSpan.FromSeconds(IDLE_POLLING_INTERVAL_SECONDS));
                }
            }
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
            if (result.Result == Result.Success)
            {
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

                    QueueRequest(request);
                }
            }
            else if (result.Result == Result.Failure)
            {
                Visit visit = _visits.AddOrUpdate(
                                            result.Endpoint,
                                            new Visit() { LastResult = Result.Failure, Visits = 1 },
                                            (key, old) =>
                                            {
                                                old.LastResult = Result.Failure;
                                                return old;
                                            });

                if (visit.Visits < FAILED_CRAWLER_RETRY_COUNT)
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
                if (visit.LastResult == Result.Failure && visit.Visits < FAILED_CRAWLER_RETRY_COUNT)
                {
                    // If we've seen the request on this enpoint previously, the only only condition 
                    // we will re-queue it is if it hasn't exhausted retries on failure

                    // ToDo: Log previous failed result and retry message.
                }
                else
                {
                    // Already visited this endpoint (either successfully or exhausted retries), don't queue it again.
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

            _queue.Enqueue(request);
        }
    }
}
