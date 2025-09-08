using BookBarn.Crawler.Utilities;
using BookBarn.Model.Api.v1;

namespace BookBarn.Crawler.GoodReads
{
    public class GoodReadsCrawlerFactory : ICrawlerFactory
    {
        private IBooksService _booksController;
        private IMediaService _mediaController;
        private IHttpClientFactory _httpClientFactory;
        private IRequestThrottle _throttle;
        private IPageClient _pageClient;

        public GoodReadsCrawlerFactory(
                                    IBooksService booksController, 
                                    IMediaService mediaController, 
                                    IHttpClientFactory httpClientFactory, 
                                    IRequestThrottle throttle, 
                                    IPageClient pageClient)
        {
            _booksController = booksController;
            _mediaController = mediaController;
            _httpClientFactory = httpClientFactory;
            _throttle = throttle;
            _pageClient = pageClient;
        }

        public Crawler Create<T>(Uri endpoint) where T : Crawler
        {
            if (typeof(T) == typeof(BookCrawler))
            {
                return new BookCrawler(endpoint, _mediaController, _booksController, _throttle, _httpClientFactory, _pageClient);
            }
            else if (typeof(T) == typeof(ListCrawler))
            {
                return new ListCrawler(endpoint, _throttle, _pageClient);
            }
            else if (typeof(T) == typeof(SeriesCrawler))
            {
                return new SeriesCrawler(endpoint, _throttle, _pageClient);
            }
            else
            {
                throw new InvalidOperationException($"Unknown crawler of type {typeof(T).FullName}");
            }
        }
    }
}
