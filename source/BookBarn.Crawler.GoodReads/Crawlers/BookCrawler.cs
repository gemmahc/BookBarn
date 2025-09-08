using BookBarn.Crawler.Utilities;
using BookBarn.Model;
using BookBarn.Model.Api.v1;

namespace BookBarn.Crawler.GoodReads
{
    [CrawlerPriority(1)]
    public class BookCrawler : Crawler
    {
        private IMediaService _mediaService;
        private IBooksService _bookService;
        private IRequestThrottle _throttle;
        private IHttpClientFactory _httpClientFactory;
        private IPageClient _pageClient;

        public BookCrawler(Uri bookPage, IMediaService mediaService, IBooksService bookService, IRequestThrottle throttle, IHttpClientFactory httpClientFactory, IPageClient pageClient) : base(bookPage)
        {
            ArgumentNullException.ThrowIfNull(bookPage);
            ArgumentNullException.ThrowIfNull(mediaService);
            ArgumentNullException.ThrowIfNull(bookService);
            ArgumentNullException.ThrowIfNull(throttle);
            ArgumentNullException.ThrowIfNull(httpClientFactory);
            ArgumentNullException.ThrowIfNull(pageClient);

            _mediaService = mediaService;
            _bookService = bookService;
            _throttle = throttle;
            _httpClientFactory = httpClientFactory;
            _pageClient = pageClient;
        }

        protected override async Task RunCrawlerAsync()
        {
            //Console.WriteLine($"Crawling: [{Endpoint}]");
            var page = new BookPage(Endpoint, _pageClient, _throttle);

            // Get the page and extract content into object.
            var book = await page.Extract();

            string bookId = book.Id ?? book.GetId();

            // If it exists, persist cover image into media storage
            if (!string.IsNullOrEmpty(book.CoverImage))
            {
                Media cover = await UploadMedia(bookId, new Uri(book.CoverImage));

                if (cover != null)
                {
                    book.CoverImage = cover.Location;
                    book.CoverMedia = cover;
                }
            }

            // Persist the book record
            Book updated = await _bookService.Put(bookId, book);

            // If the book is part of a series, dispatch a series crawler to get the other books in the series.
            if (book.SeriesUrl != null)
            {
                DispatchChild<SeriesCrawler>(new Uri(book.SeriesUrl));
            }
        }

        private async Task<Media> UploadMedia(string id, Uri bookCoverSource)
        {
            // Get the storage token from API
            var token = await _mediaService.GetWriteToken(id);

            if (token == null)
            {
                throw new Exception();
            }

            // Get the content from source
            var httpClient = _httpClientFactory.CreateClient();
            var sourceRes = await httpClient.GetAsync(bookCoverSource);
            sourceRes.EnsureSuccessStatusCode();

            // Put the content into storage
            using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Put, token.StorageEndpoint))
            {
                foreach (var kvp in token.Headers ?? [])
                {
                    req.Headers.Add(kvp.Key, kvp.Value);
                }

                req.Content = new StreamContent(sourceRes.Content.ReadAsStream());
                req.Content.Headers.ContentType = sourceRes.Content.Headers.ContentType;

                var putRes = await httpClient.SendAsync(req);
                putRes.EnsureSuccessStatusCode();
            }

            // Return the media metadata
            return await _mediaService.Get(id);
        }
    }
}
