namespace BookBarn.Crawler
{
    /// <summary>
    /// Represents a request to crawl a specified endpoint.
    /// </summary>
    public class CrawlRequest
    {
        public CrawlRequest(Uri endpoint, Type crawlerType)
        {
            ArgumentNullException.ThrowIfNull(endpoint);
            ArgumentNullException.ThrowIfNull(crawlerType);

            if (!typeof(Crawler).IsAssignableFrom(crawlerType))
            {
                throw new ArgumentException($"{nameof(crawlerType)} must inherit {typeof(Crawler).FullName}");
            }

            Endpoint = endpoint;
            RequestedCrawler = crawlerType;
        }

        /// <summary>
        /// The endpoint to crawl.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// The type of the crawler handling this request.
        /// </summary>
        public Type RequestedCrawler { get; set; }
    }
}
