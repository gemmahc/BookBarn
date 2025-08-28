namespace BookBarn.Crawler
{
    /// <summary>
    /// Provides an interface into the queue of crawlers.
    /// </summary>
    public interface ICrawlerQueue
    {
        /// <summary>
        /// Enqueue a request to crawl an endpoint.
        /// </summary>
        /// <param name="request">The crawler request.</param>
        /// <returns>Waitable task.</returns>
        public Task Enqueue(CrawlRequest request);

        /// <summary>
        /// Gets whethere there are any pending requests in the queue.
        /// </summary>
        /// <returns>True if there are pending requests.</returns>
        public Task<bool> HasWork();

        /// <summary>
        /// Gets the next request in the queue.
        /// </summary>
        /// <returns>The next request in the queue. Null if no work to do.</returns>
        public Task<CrawlRequest?> GetNext();
    }
}
