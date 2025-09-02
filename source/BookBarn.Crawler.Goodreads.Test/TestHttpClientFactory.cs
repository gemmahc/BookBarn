namespace BookBarn.Crawler.GoodReads.Test
{
    public class TestHttpClientFactory : IHttpClientFactory
    {
        public HttpClient CreateClient(string name)
        {
            return new HttpClient();
        }
    }
}
