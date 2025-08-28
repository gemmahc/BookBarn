using BookBarn.Crawler.Extensions;
using BookBarn.Crawler.Utilities;
using BookBarn.Model;
using HtmlAgilityPack;

namespace BookBarn.Crawler.GoodReads
{
    public class ListPage : Page<BookList>
    {
        public ListPage(Uri endpoint, IRequestThrottle throttle) : base(endpoint, throttle)
        { }

        public ListPage(Uri endpoint, HtmlDocument document) : base(endpoint, document)
        { }

        protected override Task<BookList> ExtractCore(HtmlDocument doc)
        {
            BookList list = new BookList();
            list.Url = Endpoint;
            string xpath = "//div[@id='all_votes']/table/tr/td/a[@class='bookTitle']";
            var listLinks = doc.DocumentNode.SelectNodes(xpath);

            if (listLinks == null || listLinks.Count == 0)
            {
                throw new PageParseException(Endpoint, $"No books links found on series page. Xpath: [{xpath}]");
            }

            foreach (var link in listLinks)
            {
                try
                {
                    string linkText = link.Attributes["href"].Value;
                    Uri book = new Uri(linkText, UriKind.Relative);
                    book = book.SetAuthorityFrom(Endpoint);

                    list.Books.Add(book);
                }
                catch (Exception ex)
                {
                    throw new PageParseException(Endpoint, $"Unexpected countent found, expecting book Uri, found [{link.InnerText}]", ex);
                }
            }

            var paginationDiv = doc.DocumentNode.SelectSingleNode("//div[@class='pagination']");

            if (paginationDiv != null)
            {
                string? currentText = paginationDiv.SelectSingleNode("./em[@class='current']")?.InnerText;
                if (!string.IsNullOrEmpty(currentText) && int.TryParse(currentText, out int pageNum))
                {
                    list.CurrentPage = pageNum;
                }

                var nextPage = paginationDiv.SelectSingleNode("./a[@class='next_page']");
                if (nextPage != null)
                {
                    string linkText = nextPage.Attributes["href"].Value;
                    Uri nextUri = new Uri(linkText, UriKind.Relative);
                    nextUri = nextUri.SetAuthorityFrom(Endpoint);
                    list.NextPage = nextUri;
                }
            }

            return Task.FromResult(list);
        }
    }
}
