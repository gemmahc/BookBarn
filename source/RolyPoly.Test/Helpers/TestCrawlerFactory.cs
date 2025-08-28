namespace RolyPoly.Test
{
    public class TestCrawlerFactory : ICrawlerFactory
    {
        public TestCrawlerFactory()
        {
            CrawlerAConstructor = new Func<Uri, TestCrawlerA>((endpoint) => { return new TestCrawlerA(endpoint); });
            CrawlerBConstructor = new Func<Uri, TestCrawlerB>((endpoint) => { return new TestCrawlerB(endpoint); });
        }

        public Crawler Create<T>(Uri endpoint) where T : Crawler
        {
            if (typeof(T) == typeof(TestCrawlerA))
            {
                return CrawlerAConstructor.Invoke(endpoint);
            }
            else if (typeof(T) == typeof(TestCrawlerB))
            {
                return CrawlerBConstructor.Invoke(endpoint);
            }
            else
            {
                throw new UnknownCrawlerTypeException(typeof(T));
            }
        }

        public Func<Uri, TestCrawlerA> CrawlerAConstructor { get; set; }

        public Func<Uri, TestCrawlerB> CrawlerBConstructor { get; set; }
    }
}
