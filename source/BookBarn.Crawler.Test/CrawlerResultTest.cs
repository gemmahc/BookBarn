namespace BookBarn.Crawler.Test
{
    public class CrawlerResultTest
    {
        [Fact]
        public void ArgumentNullCtor()
        {
            Assert.Throws<ArgumentNullException>(() => new CrawlerResult(null, Result.Success, typeof(TestCrawlerA)));

            Assert.Throws<ArgumentNullException>(() => new CrawlerResult(new Uri("https://github.com"), Result.Success, null));
        }

        [Fact]
        public void ConstructorValidProps()
        {
            Uri uri = new Uri("https://goodreads.com");
            Type type = typeof(TestCrawlerA);
            Result res = Result.Success;

            var crawlResult = new CrawlerResult(uri, res, type);

            Assert.Equal(uri, crawlResult.Endpoint);
            Assert.Equal(res, crawlResult.Result);
            Assert.Equal(type, crawlResult.CrawlerType);
            Assert.NotNull(crawlResult.ToDispatch);
        }
    }
}
