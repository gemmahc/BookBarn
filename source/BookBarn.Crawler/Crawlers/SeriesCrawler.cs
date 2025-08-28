using BookBarn.Crawler.Utilities;
using BookBarn.Model;

namespace BookBarn.Crawler.GoodReads
{
    public class SeriesCrawler : Crawler
    {
        private PartitionedRequestThrottle _throttle;

        public SeriesCrawler(Uri entrypoint, PartitionedRequestThrottle throttle) : base(entrypoint)
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
