using BookBarn.Crawler.GoodReads;
using HtmlAgilityPack;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class ListPageTest
    {
        [Fact]
        public async Task ExtractsBooksOnShortList()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.ShortList);

            ListPage page = new ListPage(TestDataHelper.ShortListUrl, doc);

            var result = await page.Extract();

            Assert.NotNull(result);

            Assert.Equal(result.CurrentPage, 1);
            Assert.Null(result.NextPage);
            Assert.Equal(75, result.Books.Count);
            Assert.Equal(TestDataHelper.ShortListUrl, result.Url);
        }

        [Fact]
        public async Task ExtractsBooksOnPaginatedList()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.FirstPageInList);

            ListPage page = new ListPage(TestDataHelper.FirstPageInListUrl, doc);

            var result = await page.Extract();

            Assert.NotNull(result);

            Assert.Equal(result.CurrentPage, 1);
            Assert.NotNull(result.NextPage);
            Assert.Equal(100, result.Books.Count);
            Assert.Equal(TestDataHelper.FirstPageInListUrl, result.Url);
        }

        [Fact]
        public async Task ExtractsBooksOnLastPageOfList()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.LastPageInList);

            ListPage page = new ListPage(TestDataHelper.LastPageInListUrl, doc);

            var result = await page.Extract();

            Assert.NotNull(result);

            Assert.Equal(result.CurrentPage, 4);
            Assert.Null(result.NextPage);
            Assert.Equal(42, result.Books.Count);
            Assert.Equal(TestDataHelper.LastPageInListUrl, result.Url);
        }
    }
}
