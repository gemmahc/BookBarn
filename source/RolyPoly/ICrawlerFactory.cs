namespace RolyPoly
{
    /// <summary>
    /// Provides an interface into creation of crawlers of known types.
    /// </summary>
    public interface ICrawlerFactory
    {
        /// <summary>
        /// Creates an instance of a crawler for the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The specific crawler type to create.</typeparam>
        /// <param name="endpoint">The endpoint to crawl.</param>
        /// <returns>The crawler instance.</returns>
        public Crawler Create<T>(Uri endpoint) where T : Crawler;
    }
}
