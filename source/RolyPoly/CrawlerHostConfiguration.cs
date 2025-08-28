namespace RolyPoly
{
    /// <summary>
    /// Crawler host configuration
    /// </summary>
    public class CrawlerHostConfiguration
    {
        private const int MAX_CONCURRENT_CRAWLERS = 10;
        private const int IDLE_POLLING_INTERVAL_MILLISECONDS = 10000;
        private const int FAILED_CRAWLER_RETRY_COUNT = 3;

        public CrawlerHostConfiguration()
        {
            MaxConcurrentCrawlers = MAX_CONCURRENT_CRAWLERS;
            IdlePollingIntervalMilliseconds = IDLE_POLLING_INTERVAL_MILLISECONDS;
            FailedCrawlerRetryCount = FAILED_CRAWLER_RETRY_COUNT;
        }

        /// <summary>
        /// Gets the maximum number of concurrent crawlers that the host will schedule at any time.
        /// </summary>
        public int MaxConcurrentCrawlers { get; set; }

        /// <summary>
        /// Gets the polling interval to check the work queue when the host is idle.
        /// </summary>
        public int IdlePollingIntervalMilliseconds { get; set; }

        /// <summary>
        /// The number of attempts to retry a failed crawler.
        /// </summary>
        public int FailedCrawlerRetryCount { get; set; }
    }
}
