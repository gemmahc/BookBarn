using BookBarn.Api.Client;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class ListCrawlerTest
    {

        //[Fact] Disabled from pipeline until properly mocked.
        public async Task HostCrawlsListSuccessfully()
        {
            Uri apiEndpoint = new Uri("http://localhost:5230");
            CrawlerQueue queue = new CrawlerQueue();

            var crawlFactory = new GoodReadsCrawlerFactory(new BookClient(apiEndpoint), new MediaClient(apiEndpoint), new TestHttpClientFactory());
            CrawlerHostConfiguration config = new CrawlerHostConfiguration()
            {
                IdlePollingIntervalMilliseconds = 1000,
                FailedCrawlerRetryCount = 0,
                MaxConcurrentCrawlers = 2
            };

            CrawlerHost host = new CrawlerHost(config, crawlFactory, queue);
            HostObserver watcher = new HostObserver();
            _ = host.Subscribe(watcher);

            await queue.Enqueue(new CrawlRequest(TestDataHelper.ShortListUrl, typeof(ListCrawler)));

            using (var source = new CancellationTokenSource(TimeSpan.FromMinutes(4)))
            {
                await host.RunAsync(source.Token);
            }
        }
    }
}
