namespace BookBarn.Model
{
    /// <summary>
    /// Represents a series of books.
    /// </summary>
    public class BookSeries
    {
        /// <summary>
        /// Creates a new instance of the <c>BookSeries</c> class.
        /// </summary>
        public BookSeries()
        {
            Books = new List<Uri>();
        }

        /// <summary>
        /// The location of the series page.
        /// </summary>
        public Uri? Url { get; set; }

        /// <summary>
        /// The list of links to the books in this series.
        /// </summary>
        public List<Uri> Books { get; set; }
    }
}
