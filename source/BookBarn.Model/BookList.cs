namespace BookBarn.Model
{
    /// <summary>
    /// Represents a curated list of books.
    /// </summary>
    public class BookList
    {
        /// <summary>
        /// Creates a new instance of the <c>BookList</c> class.
        /// </summary>
        public BookList()
        {
            Books = new List<Uri>();
            CurrentPage = 1;
        }

        /// <summary>
        /// Gets or sets the url of the book list page.
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// Gets or sets the list of books on this page.
        /// </summary>
        public List<Uri> Books { get; set; }

        /// <summary>
        /// Gets or sets the current page of result in the list.
        /// </summary>
        public int? CurrentPage { get; set; }

        /// <summary>
        /// Gets or sets the next page in a paginated book list result.
        /// </summary>
        public Uri? NextPage { get; set; }

    }
}
