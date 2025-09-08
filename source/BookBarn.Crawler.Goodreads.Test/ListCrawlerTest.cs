using HtmlAgilityPack;
using Moq;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class ListCrawlerTest
    {
        [Fact]
        public void ListCrawlerCtorValid()
        {
            ListCrawler crawler = new ListCrawler(TestDataHelper.ShortListUrl, Utilities.GetThrottle(), new Mock<IPageClient>().Object);

            Assert.Equal(TestDataHelper.ShortListUrl, crawler.Endpoint);

            Assert.Throws<ArgumentNullException>(() => new ListCrawler(null, Utilities.GetThrottle(), new Mock<IPageClient>().Object));

            Assert.Throws<ArgumentNullException>(() => new ListCrawler(TestDataHelper.ShortListUrl, null, new Mock<IPageClient>().Object));

            Assert.Throws<ArgumentNullException>(() => new ListCrawler(TestDataHelper.ShortListUrl, Utilities.GetThrottle(), null));
        }

        [Fact]
        public async Task ExtractsBooksFromListPageEndsSuccessfullyWithChildren()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.ShortList);
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.ShortListUrl)).ReturnsAsync(doc);

            ListCrawler crawler = new ListCrawler(TestDataHelper.ShortListUrl, Utilities.GetThrottle(), pageClientMock.Object);

            var result = await crawler.RunAsync();

            Assert.Equal(Result.Success, result.Result);
            Assert.Equal(75, result.ToDispatch.Count);
            Assert.Equal(TestDataHelper.ShortListUrl, result.Endpoint);
            Assert.Equal(typeof(ListCrawler), result.CrawlerType);
            Assert.All(result.ToDispatch, b => Assert.Equal(typeof(BookCrawler), b.Value));
        }

        [Fact]
        public async Task InvalidPageReturnsFailureResult()
        {
            HtmlDocument doc = new HtmlDocument();
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.ShortListUrl)).ReturnsAsync(doc);

            ListCrawler crawler = new ListCrawler(TestDataHelper.ShortListUrl, Utilities.GetThrottle(), pageClientMock.Object);

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Failure, result.Result);
            Assert.Empty(result.ToDispatch);
            Assert.Equal(TestDataHelper.ShortListUrl, result.Endpoint);
            Assert.Equal(typeof(ListCrawler), result.CrawlerType);
            Assert.IsType<PageParseException>(result.Error);
        }
    }
}
