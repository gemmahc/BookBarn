using BookBarn.Crawler.Utilities;
using BookBarn.Api.v1;

namespace BookBarn.Crawler.GoodReads
{
    public class GoodReadsCrawlerFactory : ICrawlerFactory
    {
        private IBooksService _booksController;
        private IMediaService _mediaController;
        private IHttpClientFactory _httpClientFactory;
        private PartitionedRequestThrottle _throttle;

        public GoodReadsCrawlerFactory(IBooksService booksController, IMediaService mediaController, IHttpClientFactory httpClientFactory)
        {
            _booksController = booksController;
            _mediaController = mediaController;
            _httpClientFactory = httpClientFactory;

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
                return new BookCrawler(endpoint, _mediaController, _booksController, _throttle, _httpClientFactory);
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
