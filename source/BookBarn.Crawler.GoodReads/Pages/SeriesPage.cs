using BookBarn.Crawler.Extensions;
using BookBarn.Crawler.Utilities;
using BookBarn.Model;
using HtmlAgilityPack;

namespace BookBarn.Crawler.GoodReads
{
    public class SeriesPage : Page<BookSeries>
    {
        public SeriesPage(Uri endpoint, IPageClient client, IRequestThrottle throttle) : base(endpoint, client, throttle)
        { }

        protected override Task<BookSeries> ExtractCore(HtmlDocument doc)
        {
            BookSeries series = new BookSeries();
            series.Url = Endpoint;

            string xpath = "//div[@class='responsiveBook']/div/div[@class='u-paddingBottomXSmall']/a";
            var bookElts = doc.DocumentNode.SelectNodes(xpath);

            if (bookElts == null || bookElts.Count == 0)
            {
                throw new PageParseException(Endpoint, $"No books links found on series page. Xpath: [{xpath}]");
            }

            foreach (var bookLink in bookElts)
            {
                try
                {
                    string bookHref = bookLink.Attributes["href"].Value;
                    Uri book = new Uri(bookHref, UriKind.Relative);
                    book = book.SetAuthorityFrom(Endpoint);
                    
                    series.Books.Add(book);
                }
                catch (Exception ex)
                {
                    throw new PageParseException(Endpoint, $"Unexpected countent found, expecting book Uri, found [{bookLink.InnerText}]", ex);
                }
            }

            return Task.FromResult(series);
        }
    }
}
