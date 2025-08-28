using HtmlAgilityPack;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class SeriesPageTest
    {
        [Fact]
        public async Task ExtractsBooksFromSeries()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.SeriesPage);

            SeriesPage page = new SeriesPage(TestDataHelper.SeriesPageUrl, doc);

            var result = await page.Extract();

            Assert.NotNull(result);
            Assert.Equal(13, result.Books.Count);
            Assert.Equal(TestDataHelper.SeriesPageUrl, result.Url);
        }
    }
}
