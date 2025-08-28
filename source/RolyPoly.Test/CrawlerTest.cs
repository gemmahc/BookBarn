namespace RolyPoly.Test
{
    public class CrawlerTest
    {
        [Fact]
        public async Task CrawlerSucceeds()
        {
            Uri endpoint = new Uri("https://github.com/gemmahc/BookBarn");
            TestCrawlerA crawler = new TestCrawlerA(endpoint);

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Success, result.Result);
            Assert.Equal(typeof(TestCrawlerA), result.CrawlerType);
            Assert.Equal(endpoint, result.Endpoint);
        }

        [Fact]
        public async Task CrawlerFails()
        {
            Uri endpoint = new Uri("https://github.com/gemmahc/BookBarn");
            TestCrawlerA crawler = new TestCrawlerA(endpoint);
            crawler.CrawlerAction = () =>
            {
                throw new Exception("Failed");
            };

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Failure, result.Result);
            Assert.Equal(typeof(TestCrawlerA), result.CrawlerType);
            Assert.Equal(endpoint, result.Endpoint);
        }

        [Fact]
        public async Task CrawlerSucceedsWithUniqueChildren()
        {
            string child1 = "https://github.com/gemmahc/BookBarn2";
            string child2 = "https://github.com/gemmahc/BookBarn3";

            TestCrawlerB crawler = new TestCrawlerB(new Uri("https://github.com/gemmahc/BookBarn"));
            crawler.ChildrenToAdd.Add(child1);
            crawler.ChildrenToAdd.Add(child2);

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Success, result.Result);
            Assert.Equal(2, result.ToDispatch.Count);
            Assert.Equal(typeof(TestCrawlerB), result.ToDispatch[new Uri(child1)]);
            Assert.Equal(typeof(TestCrawlerB), result.ToDispatch[new Uri(child2)]);
        }

        [Fact]
        public async Task CrawlerSucceedsWithDuplicateChildren()
        {
            string child1 = "https://github.com/gemmahc/BookBarn2";
            string child2 = "https://github.com/gemmahc/BookBarn2";

            TestCrawlerB crawler = new TestCrawlerB(new Uri("https://github.com/gemmahc/BookBarn"));
            crawler.ChildrenToAdd.Add(child1);
            crawler.ChildrenToAdd.Add(child2);

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Success, result.Result);
            Assert.Single(result.ToDispatch);
            Assert.Equal(typeof(TestCrawlerB), result.ToDispatch[new Uri(child2)]);
        }

        [Fact]
        public async Task CrawlerFailsWithAmbiguousChildren()
        {
            string child = "https://github.com/gemmahc/BookBarn2";

            TestCrawlerA crawler = new TestCrawlerA(new Uri("https://github.com/gemmahc/BookBarn"));
            crawler.ChildrenToAdd.Add(child);
            crawler.ChildrenToAdd.Add(child);

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Failure, result.Result);
            Assert.Empty(result.ToDispatch);
        }
    }
}