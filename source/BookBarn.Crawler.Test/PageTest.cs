using BookBarn.Crawler.Test.Helpers;
using BookBarn.Crawler.Utilities;
using HtmlAgilityPack;
using Moq;


namespace BookBarn.Crawler.Test
{
    public class PageTest
    {
        [Fact]
        public void PageCtorTest()
        {
            Uri sample = new Uri("https://example.com");
            TestPage page = new TestPage(sample, new Mock<IPageClient>().Object, new Mock<IRequestThrottle>().Object);

            Assert.Equal(sample, page.Endpoint);
            Assert.False(page.Initialized);

            Assert.Throws<ArgumentNullException>(() => new TestPage(null, new Mock<IPageClient>().Object, new Mock<IRequestThrottle>().Object));
            Assert.Throws<ArgumentNullException>(() => new TestPage(sample, null, new Mock<IRequestThrottle>().Object));
            Assert.Throws<ArgumentNullException>(() => new TestPage(sample, new Mock<IPageClient>().Object, null));
        }

        [Fact]
        public async Task PageExtractSetsInitProperty()
        {
            Uri sample = new Uri("https://example.com");
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(sample)).ReturnsAsync(new HtmlDocument());

            TestPage page = new TestPage(sample, pageClientMock.Object, Utilities.GetThrottle());

            Assert.Equal(sample, page.Endpoint);
            Assert.False(page.Initialized);

            var testItem = await page.Extract();

            Assert.True(page.Initialized);
        }

        [Fact]
        public async Task PageExtractCatchesParseException()
        {
            string exMsg = "Test exception";
            Uri sample = new Uri("https://example.com");
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(sample)).ReturnsAsync(new HtmlDocument());

            TestPage page = new TestPage(sample, pageClientMock.Object, Utilities.GetThrottle());
            page.Action = new Func<HtmlDocument, TestItem>(doc => throw new PageParseException(sample, exMsg));

            try
            {
                var item = await page.Extract();
                Assert.Fail("Expecting page.Extract to throw exception");
            }
            catch (PageParseException ex)
            {
                Assert.Null(ex.InnerException);
                Assert.Equal(exMsg, ex.Message);
            }
        }

        [Fact]
        public async Task PageExtractCatchesExceptionAndWrapsInParseException()
        {
            string exMsg = "Test exception";
            Uri sample = new Uri("https://example.com");
            var pageClientMock = new Mock<IPageClient>();
            pageClientMock.Setup(s => s.LoadAsync(sample)).ReturnsAsync(new HtmlDocument());

            TestPage page = new TestPage(sample, pageClientMock.Object, Utilities.GetThrottle());
            page.Action = new Func<HtmlDocument, TestItem>(doc => throw new InvalidOperationException(exMsg));

            try
            {
                var item = await page.Extract();
                Assert.Fail("Expecting page.Extract to throw exception");
            }
            catch (PageParseException ex)
            {
                Assert.NotNull(ex.InnerException);
                Assert.IsType<InvalidOperationException>(ex.InnerException);
            }
        }

        [Fact]
        public async Task PageInitFailsOnNullDocument()
        {
            Uri sample = new Uri("https://example.com");
            var pageClientMock = new Mock<IPageClient>();
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
            pageClientMock.Setup(s => s.LoadAsync(sample)).ReturnsAsync(default(HtmlDocument));
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

            TestPage page = new TestPage(sample, pageClientMock.Object, new Mock<IRequestThrottle>().Object);

            await Assert.ThrowsAsync<PageParseException>(async () => await page.Extract());
        }
    }
}
