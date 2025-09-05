namespace BookBarn.Crawler.Test
{
    internal class TestCrawlerQueue : ICrawlerQueue
    {
        Queue<CrawlRequest> _queue = new Queue<CrawlRequest>();

        public Task Enqueue(CrawlRequest request)
        {
            _queue.Enqueue(request);
            return Task.CompletedTask;
        }

        public Task<CrawlRequest?> GetNext()
        {
            return Task.Run<CrawlRequest?>(() =>
            {
                if (_queue.Any())
                {
                    return _queue.Dequeue();
                }
                else
                {
                    return null;
                }
            });
        }

        public Task<bool> HasWork()
        {
            return Task.Run<bool>(() =>
            {
                return _queue.Any();
            });
        }

        public Task<int> Count()
        {
            return Task.FromResult(_queue.Count());
        }
    }
}
