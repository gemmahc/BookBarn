namespace BookBarn.Crawler.Host
{
    public class InMemoryQueue : ICrawlerQueue
    {
        PriorityQueue<CrawlRequest, int> _queue;
        object _lock;

        public InMemoryQueue()
        {
            _lock = new object();
            _queue = new PriorityQueue<CrawlRequest, int>();
        }

        public Task Enqueue(CrawlRequest request)
        {
            CrawlerPriorityAttribute? priorityAttr = Attribute.GetCustomAttribute(request.RequestedCrawler, typeof(CrawlerPriorityAttribute)) as CrawlerPriorityAttribute;

            int priority = priorityAttr?.Priority ?? 1;

            lock (_lock)
            {
                _queue.Enqueue(request, priority);
            }

            return Task.CompletedTask;
        }

        public Task<CrawlRequest?> GetNext()
        {
            CrawlRequest? request = null;

            lock (_lock)
            {
                if (_queue.Count > 0)
                {
                    request = _queue.Dequeue();
                }
            }

            return Task.FromResult(request);
        }

        public Task<bool> HasWork()
        {
            lock (_lock)
            {
                return Task.FromResult(_queue.Count > 0);
            }
        }

        public Task<int> Count()
        {
            lock(_lock)
            {
                return Task.FromResult(_queue.Count);
            }
        }
    }
}
