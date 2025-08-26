namespace RolyPoly
{
    /// <summary>
    /// Represents the result of a crawler execution.
    /// </summary>
    public class CrawlerResult
    {
        public CrawlerResult(Uri endpoint, Result result, Type crawlerType)
        {
            ArgumentNullException.ThrowIfNull(endpoint);
            ArgumentNullException.ThrowIfNull(crawlerType);

            ToDispatch = new Dictionary<Uri, Type>();
            CrawlerType = crawlerType;
            Endpoint = endpoint;
            Result = result;
        }

        /// <summary>
        /// Gets or sets the endpoint that was crawled.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the type of crawler used.
        /// </summary>
        public Type CrawlerType { get; set; }

        /// <summary>
        /// The final result of the crawler's execution.
        /// </summary>
        public Result Result { get; set; }

        /// <summary>
        /// Crawlers requested by the execution of this crawler.
        /// </summary>
        public IDictionary<Uri, Type> ToDispatch { get; private set; }
    }
}
