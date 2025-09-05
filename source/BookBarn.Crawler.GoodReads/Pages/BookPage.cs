using BookBarn.Crawler.Utilities;
using BookBarn.Model;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace BookBarn.Crawler.GoodReads
{
    /// <summary>
    /// Encapsulates extraction for a GoodReads book page https://www.goodreads.com/book
    /// </summary>
    public class BookPage : Page<Book>
    {
        /// <summary>
        /// Creates a new BookPage for the specified endpoint.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="throttle">The request throttle used when accessing the endpoint.</param>
        public BookPage(Uri endpoint, IRequestThrottle throttle) : base(endpoint, throttle)
        { }

        /// <summary>
        /// Creates a new BookPage for the specified endpoint, prepopulated with the provided html doc.
        /// </summary>
        /// <param name="endpoint">The endpoint.</param>
        /// <param name="document">The page html.</param>
        public BookPage(Uri endpoint, HtmlDocument document) : base(endpoint, document)
        {
        }

        /// <summary>
        /// Extraction logic for HTML into Book
        /// </summary>
        /// <param name="doc">The HTML document to process.</param>
        /// <returns>The book instance.</returns>
        /// <exception cref="PageParseException">Thrown when unable to identify/parse page contents.</exception>
        protected override Task<Book> ExtractCore(HtmlDocument doc)
        {
            Book book = new Book();

            book.Url = Endpoint.ToString();

            string xpath = "//script[@id='__NEXT_DATA__' and @type='application/json']/text()";
            string? pageData = doc.DocumentNode.SelectSingleNode(xpath)?.InnerText;

            if (string.IsNullOrEmpty(pageData))
            {
                throw new PageParseException(Endpoint, $"Unable to locate book page json at xpath [{xpath}]");
            }

            dynamic? _bookPageJson = JsonConvert.DeserializeObject(pageData);

            if (_bookPageJson == null)
            {
                throw new PageParseException(Endpoint, $"Book page content at [{xpath}] is not valid JSON");
            }

            dynamic? cont = _bookPageJson["props"]["pageProps"]["apolloState"];

            if (cont == null)
            {
                throw new PageParseException(Endpoint, $"Book page content at [{xpath}] has unexpected JSON");
            }

            foreach (var item in cont)
            {
                string? key = item.Name as string;

                if (string.IsNullOrEmpty(key)) continue;
                else if (key.StartsWith("Book:"))
                {
                    book.Title = item.Value["title"];

                    if(string.IsNullOrEmpty(book.Title))
                    {
                        // Not the complete book entry.
                        // Continue and encounter the complete entry later or fail at the end with incomplete book details.
                        continue;
                    }

                    book.Description = item.Value["description"];
                    book.CoverImage = item.Value["imageUrl"];
                    book.Format = item.Value["details"]["format"];
                    book.Pages = item.Value["details"]["numPages"];

                    if (item.Value["bookSeries"].Count > 0)
                    {
                        book.SeriesRank = item.Value["bookSeries"][0]["userPosition"];
                    }

                    // Populate genres
                    List<string> genres = new List<string>();
                    foreach (var genre in item.Value["bookGenres"])
                    {
                        string name = genre["genre"]["name"];
                        if (!string.IsNullOrEmpty(name))
                        {
                            genres.Add(name);
                        }
                    }
                    if (genres.Any())
                    {
                        book.Genres = genres.ToArray();
                    }

                    string? authorRef = item.Value["primaryContributorEdge"]["node"]["__ref"];
                    if (!string.IsNullOrEmpty(authorRef))
                    {
                        book.Author = cont[authorRef]["name"];
                    }

                    // Publish date fallback (current edition) if not in the Work section (first edition)
                    long? pubTime = item.Value["details"]["publicationTime"];
                    if (pubTime.HasValue && book.PublishDate == null)
                    {
                        DateTime pubDate = DateTime.UnixEpoch.AddMilliseconds(pubTime.Value);

                        book.PublishDate = pubDate;
                        book.PublishYear = pubDate.Year;
                    }
                }
                else if (key.StartsWith("Series:"))
                {
                    book.SeriesName = item.Value["title"];
                    book.SeriesUrl = item.Value["webUrl"];
                }
                else if (key.StartsWith("Work:"))
                {
                    book.Rating = item.Value["stats"]["averageRating"];
                    book.RatingCount = item.Value["stats"]["ratingsCount"];

                    long? pubTime = item.Value["details"]["publicationTime"];
                    if (pubTime.HasValue)
                    {
                        DateTime pubDate = DateTime.UnixEpoch.AddMilliseconds(pubTime.Value);

                        book.PublishDate = pubDate;
                        book.PublishYear = pubDate.Year;
                    }
                }
                else if (key.StartsWith("Contributor:"))
                {
                    string? authorFallback = item.Value["name"];
                    if (string.IsNullOrEmpty(book.Author) && !string.IsNullOrEmpty(authorFallback))
                    {
                        book.Author = authorFallback;
                    }
                }
            }

            // Sanity check on parsed book. Title and Author are minimum required for a uniquely identified book.
            if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author))
            {
                throw new PageParseException(Endpoint, $"Title or Author not found on page [{Endpoint}]. Invalid book.");
            }

            book.Id = book.GetId();

            return Task.FromResult(book);
        }
    }
}
