using HtmlAgilityPack;

namespace BookBarn.Crawler
{
    /// <summary>
    /// Implementation of IPageClient using HtmlAgilityPack's HtmlWeb class.
    /// </summary>
    public class HtmlPageClient : IPageClient
    {
        /// <summary>
        /// Load the html document from the specified address.
        /// </summary>
        /// <param name="pageAddress">The page address to load.</param>
        /// <returns>The HtmlDocument.</returns>
        public async Task<HtmlDocument> LoadAsync(Uri pageAddress)
        {
            // HtmlAgilityPack uses a shared HttpClient dictionary for requests.
            // However, HtmlWeb is not thread safe and hangs on to items from each request
            // so we cannot reuse it.
            var web = new HtmlWeb();
            return await web.LoadFromWebAsync(pageAddress.ToString());
        }
    }
}
