using HtmlAgilityPack;
using Moq;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class SeriesCrawlerTest
    {
        [Fact]
        public void SeriesCrawlerCtorValid()
        {
            SeriesCrawler crawler = new SeriesCrawler(TestDataHelper.SeriesPageUrl, Utilities.GetThrottle(), new Mock<IPageClient>().Object);

            Assert.Equal(TestDataHelper.SeriesPageUrl, crawler.Endpoint);

            Assert.Throws<ArgumentNullException>(() => new SeriesCrawler(null, Utilities.GetThrottle(), new Mock<IPageClient>().Object));

            Assert.Throws<ArgumentNullException>(() => new SeriesCrawler(TestDataHelper.SeriesPageUrl, null, new Mock<IPageClient>().Object));

            Assert.Throws<ArgumentNullException>(() => new SeriesCrawler(TestDataHelper.SeriesPageUrl, Utilities.GetThrottle(), null));
        }

        [Fact]
        public async Task ExtractsBooksFromSeriesPageEndsSuccessfullyWithChildren()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.SeriesPage);
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.SeriesPageUrl)).ReturnsAsync(doc);

            SeriesCrawler crawler = new SeriesCrawler(TestDataHelper.SeriesPageUrl, Utilities.GetThrottle(), pageClientMock.Object);

            var result = await crawler.RunAsync();

            Assert.Equal(Result.Success, result.Result);
            Assert.Equal(13, result.ToDispatch.Count);
            Assert.Equal(TestDataHelper.SeriesPageUrl, result.Endpoint);
            Assert.Equal(typeof(SeriesCrawler), result.CrawlerType);
            Assert.All(result.ToDispatch, b => Assert.Equal(typeof(BookCrawler), b.Value));
        }

        [Fact]
        public async Task ThrowsPageParseExceptionOnInvalidPage()
        {
            HtmlDocument doc = new HtmlDocument();
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.SeriesPageUrl)).ReturnsAsync(doc);

            SeriesCrawler crawler = new SeriesCrawler(TestDataHelper.SeriesPageUrl, Utilities.GetThrottle(), pageClientMock.Object);

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Failure, result.Result);
            Assert.Empty(result.ToDispatch);
            Assert.Equal(TestDataHelper.SeriesPageUrl, result.Endpoint);
            Assert.Equal(typeof(SeriesCrawler), result.CrawlerType);
            Assert.IsType<PageParseException>(result.Error);
        }
    }
}
