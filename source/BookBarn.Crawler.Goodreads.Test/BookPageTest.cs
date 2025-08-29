using BookBarn.Model;
using HtmlAgilityPack;

namespace BookBarn.Crawler.GoodReads.Test
{
    public class BookPageTest
    {
        [Fact]
        public async Task ExtractsBookInSeries()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.BookInSeries);
            BookPage page = new BookPage(TestDataHelper.BookInSeriesUrl, doc);

            Book res = await page.Extract();

            Assert.NotNull(res);

            // Assert parsed fields are populated.
            Assert.Equal("The Fellowship of the Ring", res.Title);
            Assert.Equal("J.R.R. Tolkien", res.Author);
            Assert.Equal("1", res.SeriesRank);
            Assert.Equal("Middle Earth", res.SeriesName);
            Assert.Equal("https://www.goodreads.com/series/66175-middle-earth", res.SeriesUrl);
            Assert.Equal(432, res.Pages);
            Assert.Equal("Kindle Edition", res.Format);
            Assert.Equal(TestDataHelper.BookInSeriesUrl.ToString(), res.Url);
            Assert.Equal("https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1654215925i/61215351.jpg", res.CoverImage);
            Assert.Equal(1954, res.PublishYear);
            Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(-486838800000), res.PublishDate);
            Assert.Equal(10, res.Genres?.Length);
            Assert.Equal(3068562, res.RatingCount);
            Assert.Equal(4.4, res.Rating);
            Assert.False(string.IsNullOrEmpty(res.Description));

            // Ensure non-parsed fields are still null.
            Assert.Null(res.CoverMedia);
            Assert.Null(res.Id);
            Assert.Null(res.LastUpdated);
        }

        [Fact]
        public async Task ExtractsStandaloneBook()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.StandaloneBook);
            BookPage page = new BookPage(TestDataHelper.StandaloneBookUri, doc);

            Book res = await page.Extract();

            Assert.NotNull(res);

            // Assert parsed fields are populated.
            Assert.Equal("Frankenstein: The 1818 Text", res.Title);
            Assert.Equal("Mary Wollstonecraft Shelley", res.Author);
            Assert.Equal(260, res.Pages);
            Assert.Equal("Paperback", res.Format);
            Assert.Equal(TestDataHelper.StandaloneBookUri.ToString(), res.Url);
            Assert.Equal("https://images-na.ssl-images-amazon.com/images/S/compressed.photo.goodreads.com/books/1631088473i/35031085.jpg", res.CoverImage);
            Assert.Equal(1818, res.PublishYear);
            Assert.Equal(DateTime.UnixEpoch.AddMilliseconds(-4796640422000), res.PublishDate);
            Assert.Equal(10, res.Genres?.Length);
            Assert.Equal(1783977, res.RatingCount);
            Assert.Equal(3.89, res.Rating);
            Assert.False(string.IsNullOrEmpty(res.Description));

            //Ensure series fields are still null.
            Assert.Null(res.SeriesName);
            Assert.Null(res.SeriesRank);
            Assert.Null(res.SeriesUrl);

            // Ensure non-parsed fields are still null.
            Assert.Null(res.CoverMedia);
            Assert.Null(res.Id);
            Assert.Null(res.LastUpdated);
        }

        [Fact]
        public async Task FailsOnMissingBookJson()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.StandaloneBook);
            string xpath = "//script[@id='__NEXT_DATA__' and @type='application/json']";
            var jsonData = doc.DocumentNode.SelectSingleNode(xpath);
            jsonData.Remove();

            BookPage page = new BookPage(TestDataHelper.StandaloneBookUri, doc);

            await Assert.ThrowsAsync<PageParseException>(async () => await page.Extract());
        }

        [Fact]
        public async Task FailsOnEmptyBookJson()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.StandaloneBook);
            string xpath = "//script[@id='__NEXT_DATA__' and @type='application/json']";
            var jsonData = doc.DocumentNode.SelectSingleNode(xpath);
            jsonData.InnerHtml = "{\"props\":{\"pageProps\":{\"apolloState\":[]}}}";

            BookPage page = new BookPage(TestDataHelper.StandaloneBookUri, doc);

            await Assert.ThrowsAsync<PageParseException>(async () => await page.Extract());
        }

        [Fact]
        public async Task FailsOnInvalidBookJson()
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(TestDataHelper.StandaloneBook);
            string xpath = "//script[@id='__NEXT_DATA__' and @type='application/json']";
            var jsonData = doc.DocumentNode.SelectSingleNode(xpath);
            jsonData.InnerHtml = "{}";

            BookPage page = new BookPage(TestDataHelper.StandaloneBookUri, doc);

            await Assert.ThrowsAsync<PageParseException>(async () => await page.Extract());
        }
    }
}