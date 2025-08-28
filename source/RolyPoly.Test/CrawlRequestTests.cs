namespace RolyPoly.Test
{
    public class CrawlRequestTests
    {
        [Fact]
        public void ExceptionOnNullCtor()
        {
            Assert.Throws<ArgumentNullException>(() => new CrawlRequest(null, typeof(TestCrawlerA)));

            Assert.Throws<ArgumentNullException>(() => new CrawlRequest(new Uri("https://github.com"), null));
        }

        [Fact]
        public void ExceptionOnNonCrawlerType()
        {
            Assert.Throws<ArgumentException>(() => new CrawlRequest(new Uri("https://github.com"), typeof(Uri)));
        }

        [Fact]
        public void ConstructorValidProps()
        {
            Uri endpoint = new Uri("https://github.com");
            Type type = typeof(TestCrawlerA);

            var req = new CrawlRequest(endpoint, type);

            Assert.Equal(endpoint, req.Endpoint);
            Assert.Equal(type, req.RequestedCrawler);
        }
    }
}
