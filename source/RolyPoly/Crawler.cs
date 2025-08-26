namespace RolyPoly
{
    /// <summary>
    /// Represents a crawler over a specified endpoint.
    /// </summary>
    public abstract class Crawler
    {
        /// <summary>
        /// Requests to traverse into children.
        /// </summary>
        private Dictionary<Uri, Type> _toDispatch;

        /// <summary>
        /// The endpoint to crawl.
        /// </summary>
        public Uri Endpoint { get; }

        protected Crawler(Uri entrypoint)
        {
            _toDispatch = new Dictionary<Uri, Type>();
            Endpoint = entrypoint;
        }

        /// <summary>
        /// Executes the crawler and returns a result.
        /// </summary>
        /// <returns></returns>
        public async Task<CrawlerResult> RunAsync()
        {
            try
            {
                await RunCrawlerAsync();

                var result = new CrawlerResult(Endpoint, Result.Success, this.GetType());

                foreach(var kvp in _toDispatch)
                {
                    result.ToDispatch.Add(kvp.Key, kvp.Value);
                }

                return result;
            }
            catch (Exception)
            {
                return new CrawlerResult(Endpoint, Result.Failure, this.GetType());
            }
        }

        /// <summary>
        /// Requests to spawn a child crawler of a specified type.
        /// </summary>
        /// <typeparam name="T">Type of crawler.</typeparam>
        /// <param name="endpoint">The location to crawl.</param>
        /// <exception cref="AmbiguiousCrawlerException">Thrown if crawler of multiple types is requested for this endpoint.</exception>
        protected void DispatchChild<T>(Uri endpoint) where T : Crawler
        {
            if(_toDispatch.ContainsKey(endpoint))
            {
                if (_toDispatch[endpoint] != typeof(T))
                {
                    throw new AmbiguiousCrawlerException($"Cannot dispatch crawler on [{endpoint}] of type [{typeof(T).Name}]. Crawler [{_toDispatch[endpoint].Name}] is already requested for that location.");
                }
            }
            else
            {
                _toDispatch.Add(endpoint, typeof(T));
            }
        }

        /// <summary>
        /// Executes crawling logic on the endpoint.
        /// </summary>
        /// <returns>Waitable task.</returns>
        protected abstract Task RunCrawlerAsync();
    }
}
