using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookBarn.Api.Client;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class BookCrawlerTest
    {
        //[Fact] Disabled from pipeline build until properly mocked.
        public async Task BookCrawlerRunsSuccessfully()
        {
            BookCrawler crawler = new BookCrawler(TestDataHelper.StandaloneBookUri,
                                                    new MediaClient(new Uri("http://localhost:5230")),
                                                    new BookClient(new Uri("http://localhost:5230")),
                                                    Utilities.GetThrottle(),
                                                    new TestHttpClientFactory()
                                                    );

            var result = await crawler.RunAsync();

            Assert.NotNull(result);
            Assert.Equal(Result.Success, result.Result);
            Assert.Empty(result.ToDispatch);
            Assert.Equal(TestDataHelper.StandaloneBookUri, result.Endpoint);
            Assert.Equal(typeof(BookCrawler), result.CrawlerType);
        }
    }
}
