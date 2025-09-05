using BookBarn.Crawler.Utilities;
using BookBarn.Model;

namespace BookBarn.Crawler.GoodReads
{
    [CrawlerPriority(5)]
    public class SeriesCrawler : Crawler
    {
        private IRequestThrottle _throttle;

        public SeriesCrawler(Uri entrypoint, IRequestThrottle throttle) : base(entrypoint)
        {
            _throttle = throttle;
        }

        protected override async Task RunCrawlerAsync()
        {
            SeriesPage page = new SeriesPage(Endpoint, _throttle);

            BookSeries series = await page.Extract();
            
            foreach(Uri book in series.Books)
            {
                DispatchChild<BookCrawler>(book);
            }

            // ToDo: Persist series information?
        }
    }
}
