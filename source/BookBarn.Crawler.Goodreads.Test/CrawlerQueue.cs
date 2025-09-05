namespace BookBarn.Crawler.GoodReads.Test
{
    internal class CrawlerQueue : ICrawlerQueue
    {
        private Queue<CrawlRequest> _queue;

        public CrawlerQueue()
        {
            _queue = new Queue<CrawlRequest>();
        }

        public Task<int> Count()
        {
            return Task.FromResult(_queue.Count);
        }

        public Task Enqueue(CrawlRequest request)
        {
            _queue.Enqueue(request);

            return Task.CompletedTask;
        }

        public Task<CrawlRequest?> GetNext()
        {
            CrawlRequest? result = null;

            _ = _queue.TryDequeue(out result);

            return Task.FromResult(result);
        }

        public Task<bool> HasWork()
        {
            return Task.FromResult(_queue.Any());
        }
    }
}
