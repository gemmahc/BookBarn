using BookBarn.Api.Client;
using BookBarn.Model.Api.v1;
using HtmlAgilityPack;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class BookCrawlerTest
    {
        [Fact]
        public void BookCrawlerCtorValidTest()
        {
            BookCrawler crawler = new BookCrawler(TestDataHelper.StandaloneBookUri, 
                new Mock<IMediaService>().Object, 
                new Mock<IBooksService>().Object, 
                Utilities.GetThrottle(), 
                new Mock<IHttpClientFactory>().Object,
                new Mock<IPageClient>().Object);

            Assert.Equal(TestDataHelper.StandaloneBookUri, crawler.Endpoint);

            Assert.Throws<ArgumentNullException>(() => new BookCrawler(null,
                new Mock<IMediaService>().Object,
                new Mock<IBooksService>().Object,
                Utilities.GetThrottle(),
                new Mock<IHttpClientFactory>().Object,
                new Mock<IPageClient>().Object));
            Assert.Throws<ArgumentNullException>(() => new BookCrawler(TestDataHelper.StandaloneBookUri,
                null,
                new Mock<IBooksService>().Object,
                Utilities.GetThrottle(),
                new Mock<IHttpClientFactory>().Object,
                new Mock<IPageClient>().Object));
            Assert.Throws<ArgumentNullException>(() => new BookCrawler(TestDataHelper.StandaloneBookUri,
                new Mock<IMediaService>().Object,
                null,
                Utilities.GetThrottle(),
                new Mock<IHttpClientFactory>().Object,
                new Mock<IPageClient>().Object));
            Assert.Throws<ArgumentNullException>(() => new BookCrawler(TestDataHelper.StandaloneBookUri,
                new Mock<IMediaService>().Object,
                new Mock<IBooksService>().Object,
                null,
                new Mock<IHttpClientFactory>().Object,
                new Mock<IPageClient>().Object));
            Assert.Throws<ArgumentNullException>(() => new BookCrawler(TestDataHelper.StandaloneBookUri,
                new Mock<IMediaService>().Object,
                new Mock<IBooksService>().Object,
                Utilities.GetThrottle(),
                null,
                new Mock<IPageClient>().Object));
            Assert.Throws<ArgumentNullException>(() => new BookCrawler(TestDataHelper.StandaloneBookUri,
               new Mock<IMediaService>().Object,
               new Mock<IBooksService>().Object,
               Utilities.GetThrottle(),
               new Mock<IHttpClientFactory>().Object,
               null));
        }

        [Fact]
        public async Task RunSuccessBookCrawlerWithSeriesChildren()
        {
            await Task.CompletedTask;
        }

        [Fact]
        public async Task RunSuccessfulBookCrawlerOnStandalone()
        {
            await Task.CompletedTask;
        }

        [Fact]
        public async Task RunFailedBookCrawlerOnInvalidPage()
        {
            await Task.CompletedTask;
        }
    }
}
