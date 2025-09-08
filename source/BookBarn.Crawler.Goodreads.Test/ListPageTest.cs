using BookBarn.Crawler.GoodReads;
using HtmlAgilityPack;
using Moq;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class ListPageTest
    {
        [Fact]
        public void ListPageCtorTest()
        {
            ListPage page = new ListPage(TestDataHelper.ShortListUrl, new Mock<IPageClient>().Object, Utilities.GetThrottle());

            Assert.Equal(TestDataHelper.ShortListUrl, page.Endpoint);
            Assert.False(page.Initialized);

            Assert.Throws<ArgumentNullException>(() => new ListPage(null, new Mock<IPageClient>().Object, Utilities.GetThrottle()));
            Assert.Throws<ArgumentNullException>(() => new ListPage(TestDataHelper.ShortListUrl, null, Utilities.GetThrottle()));
            Assert.Throws<ArgumentNullException>(() => new ListPage(TestDataHelper.ShortListUrl, new Mock<IPageClient>().Object, null));
        }

        [Fact]
        public async Task ExtractsBooksOnShortList()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.ShortList);

            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.ShortListUrl)).ReturnsAsync(doc);

            ListPage page = new ListPage(TestDataHelper.ShortListUrl, pageClientMock.Object, Utilities.GetThrottle());

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
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.FirstPageInListUrl)).ReturnsAsync(doc);

            ListPage page = new ListPage(TestDataHelper.FirstPageInListUrl, pageClientMock.Object, Utilities.GetThrottle());

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

            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.LastPageInListUrl)).ReturnsAsync(doc);

            ListPage page = new ListPage(TestDataHelper.LastPageInListUrl, pageClientMock.Object, Utilities.GetThrottle());

            var result = await page.Extract();

            Assert.NotNull(result);

            Assert.Equal(result.CurrentPage, 4);
            Assert.Null(result.NextPage);
            Assert.Equal(42, result.Books.Count);
            Assert.Equal(TestDataHelper.LastPageInListUrl, result.Url);
        }
    }
}
