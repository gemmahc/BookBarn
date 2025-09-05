using BookBarn.Crawler.Utilities;
using BookBarn.Model;

namespace BookBarn.Crawler.GoodReads
{
    [CrawlerPriority(10)]
    public class ListCrawler : Crawler
    {
        IRequestThrottle _throttle;

        public ListCrawler(Uri entrypoint, IRequestThrottle throttle) : base(entrypoint)
        {
            _throttle = throttle;
        }

        protected override async Task RunCrawlerAsync()
        {
            ListPage page = new ListPage(Endpoint, _throttle);

            BookList list = await page.Extract();

            // If list is paginated, dispatch crawler for the next page of the list.
            if(list.NextPage != null)
            {
                DispatchChild<ListCrawler>(list.NextPage);
            }

            // Dispatch crawlers for each of the books linked on the page.
            foreach (var bookUri in list.Books)
            {
                DispatchChild<BookCrawler>(bookUri);
            }

            // ToDo: Persist book list information?
        }
    }
}
