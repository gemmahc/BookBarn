using BookBarn.Crawler.Utilities;
using BookBarn.Model.Providers;

namespace BookBarn.Crawler.GoodReads
{
    internal class GoodReadsCrawlerFactory : ICrawlerFactory
    {
        private IBookDataProvider _bookDataProvider;
        private IMediaStorageProvider _mediaStorageProvider;
        private PartitionedRequestThrottle _throttle;

        public GoodReadsCrawlerFactory(IBookDataProvider bookProvider, IMediaStorageProvider mediaProvider)
        {
            _bookDataProvider = bookProvider;
            _mediaStorageProvider = mediaProvider;

            RequestThrottleOptions throttleOpts = new RequestThrottleOptions()
            {
                MaxConcurrentRequests = 2,
                MaxQueuedRequests = 10000,
                Interval = TimeSpan.FromSeconds(1)
            };
            _throttle = new PartitionedRequestThrottle(throttleOpts);
        }

        public Crawler Create<T>(Uri endpoint) where T : Crawler
        {
            if (typeof(T) == typeof(BookCrawler))
            {
                return new BookCrawler(endpoint, _mediaStorageProvider, _bookDataProvider, _throttle);
            }
            else if (typeof(T) == typeof(ListCrawler))
            {
                return new ListCrawler(endpoint, _throttle);
            }
            else if (typeof(T) == typeof(SeriesCrawler))
            {
                return new SeriesCrawler(endpoint, _throttle);
            }
            else
            {
                throw new InvalidOperationException($"Unknown crawler of type {typeof(T).FullName}");
            }
        }
    }
}
