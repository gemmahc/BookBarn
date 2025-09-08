using HtmlAgilityPack;
using Moq;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class SeriesPageTest
    {
        [Fact]
        public void SeriesPageCtorTest()
        {
            SeriesPage page = new SeriesPage(TestDataHelper.SeriesPageUrl, new Mock<IPageClient>().Object, Utilities.GetThrottle());

            Assert.Equal(TestDataHelper.SeriesPageUrl, page.Endpoint);
            Assert.False(page.Initialized);

            Assert.Throws<ArgumentNullException>(() => new SeriesPage(null, new Mock<IPageClient>().Object, Utilities.GetThrottle()));
            Assert.Throws<ArgumentNullException>(() => new SeriesPage(TestDataHelper.SeriesPageUrl, null, Utilities.GetThrottle()));
            Assert.Throws<ArgumentNullException>(() => new SeriesPage(TestDataHelper.SeriesPageUrl, new Mock<IPageClient>().Object, null));
        }

        [Fact]
        public async Task ExtractsBooksFromSeries()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.SeriesPage);

            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(TestDataHelper.SeriesPageUrl)).ReturnsAsync(doc);

            SeriesPage page = new SeriesPage(TestDataHelper.SeriesPageUrl, pageClientMock.Object, Utilities.GetThrottle());

            var result = await page.Extract();

            Assert.NotNull(result);
            Assert.Equal(13, result.Books.Count);
            Assert.Equal(TestDataHelper.SeriesPageUrl, result.Url);
        }
    }
}
