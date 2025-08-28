using BookBarn.Crawler.Utilities;
using BookBarn.Model;
using BookBarn.Model.Providers;

namespace BookBarn.Crawler.GoodReads
{
    public class BookCrawler : Crawler
    {
        private IMediaStorageProvider _mediaProvider;
        private IBookDataProvider _bookProvider;
        private IRequestThrottle _throttle;

        public BookCrawler(Uri bookPage, IMediaStorageProvider mediaProvider, IBookDataProvider bookProvider, IRequestThrottle throttle) : base(bookPage)
        {
            _mediaProvider = mediaProvider;
            _bookProvider = bookProvider;
            _throttle = throttle;
        }

        protected override async Task RunCrawlerAsync()
        {
            var page = new BookPage(Endpoint, _throttle);

            // Get the page and extract content into object.
            var book = await page.Extract();

            // If it exists, persist cover image into media storage
            if (!string.IsNullOrEmpty(book.CoverImage))
            {
                Media cover = await _mediaProvider.UpsertFrom(new Uri(book.CoverImage));

                book.CoverImage = cover.Location;
                book.CoverMedia = cover;
            }

            // Persist the book record
            await _bookProvider.Upsert(book);

            // If the book is part of a series, dispatch a series crawler to get the other books in the series.
            if(book.SeriesUrl != null)
            {
                DispatchChild<SeriesCrawler>(new Uri(book.SeriesUrl));
            }
        }
    }
}
