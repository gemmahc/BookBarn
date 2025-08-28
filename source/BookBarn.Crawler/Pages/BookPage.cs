using BookBarn.Crawler.Utilities;
using BookBarn.Model;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace BookBarn.Crawler.GoodReads
{
    public class BookPage : Page<Book>
    {
        public BookPage(Uri endpoint, IRequestThrottle throttle) : base(endpoint, throttle)
        { }

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

            Console.WriteLine(pageData);
            dynamic? _bookPageJson = JsonConvert.DeserializeObject(pageData);

            if (_bookPageJson == null)
            {
                throw new PageParseException(Endpoint, $"Book page content at [{xpath}] is not valid JSON");
            }

            var cont = _bookPageJson["props"]["pageProps"]["apolloState"];

            foreach (var item in cont)
            {
                string? key = item.Name as string;
                Console.WriteLine(key);
                if (string.IsNullOrEmpty(key)) continue;
                else if (key.StartsWith("Book:"))
                {
                    book.Title = item.Value["title"];
                    book.Description = item.Value["description"];
                    book.CoverImage = item.Value["imageUrl"];
                    book.SeriesRank = item.Value["bookSeries"][0]["userPosition"];
                    book.Format = item.Value["details"]["format"];
                    book.Pages = item.Value["details"]["numPages"];

                    long pubTime = item.Value["details"]["publicationTime"];
                    DateTime pubDate = DateTime.UnixEpoch.AddMilliseconds(pubTime);

                    book.PublishDate = pubDate;
                    book.PublishYear = pubDate.Year;

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

            return Task.FromResult(book);
        }
    }
}
