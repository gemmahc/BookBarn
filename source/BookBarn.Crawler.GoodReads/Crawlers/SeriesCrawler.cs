using BookBarn.Crawler.Utilities;
using BookBarn.Model;

namespace BookBarn.Crawler.GoodReads
{
    [CrawlerPriority(5)]
    public class SeriesCrawler : Crawler
    {
        private IRequestThrottle _throttle;
        private IPageClient _pageClient;

        public SeriesCrawler(Uri entrypoint, IRequestThrottle throttle, IPageClient pageClient) : base(entrypoint)
        {
            ArgumentNullException.ThrowIfNull(entrypoint);
            ArgumentNullException.ThrowIfNull(throttle);
            ArgumentNullException.ThrowIfNull(pageClient);

            _throttle = throttle;
            _pageClient = pageClient;
        }

        protected override async Task RunCrawlerAsync()
        {
            SeriesPage page = new SeriesPage(Endpoint, _pageClient, _throttle);

            BookSeries series = await page.Extract();

            foreach (Uri book in series.Books)
            {
                DispatchChild<BookCrawler>(book);
            }

            // ToDo: Persist series information?
        }
    }
}
