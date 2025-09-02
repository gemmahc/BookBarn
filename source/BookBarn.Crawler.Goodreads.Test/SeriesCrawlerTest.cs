using BookBarn.Api.Client;

namespace BookBarn.Crawler.GoodReads.Test
{

    public class SeriesCrawlerTest
    {
        // [Fact] Disabled from pipeline until properly mocked.
        public async Task HostCrawlsSeriesSuccessfully()
        {
            Uri apiEndpoint = new Uri("http://localhost:5230");
            CrawlerQueue queue = new CrawlerQueue();

            var crawlFactory = new GoodReadsCrawlerFactory(new BookClient(apiEndpoint), new MediaClient(apiEndpoint), new TestHttpClientFactory());
            CrawlerHostConfiguration config = new CrawlerHostConfiguration()
            {
                IdlePollingIntervalMilliseconds = 1000,
                FailedCrawlerRetryCount = 1,
                MaxConcurrentCrawlers = 2
            };

            CrawlerHost host = new CrawlerHost(config, crawlFactory, queue);

            await queue.Enqueue(new CrawlRequest(TestDataHelper.SeriesPageUrl, typeof(SeriesCrawler)));

            using (var source = new CancellationTokenSource(TimeSpan.FromMinutes(2)))
            {
                await host.RunAsync(source.Token);
            }
        }
    }
}
