namespace BookBarn.Crawler
{
    /// <summary>
    /// Exception thrown when parsing a page encounters a problem.
    /// </summary>
    public class PageParseException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <c>PageParseException</c> class
        /// </summary>
        public PageParseException() : base()
        {
        }

        /// <summary>
        /// Creates a new instance of the <c>PageParseException</c> class for the specified page and message.
        /// </summary>
        /// <param name="page">The page Uri.</param>
        /// <param name="message">The error message.</param>
        public PageParseException(Uri page, string? message) : base(message)
        {
            Page = page;
        }

        /// <summary>
        /// Creates a new instance of the <c>PageParseException</c> class for the specified page, message and captured exception.
        /// </summary>
        /// <param name="page">The page Uri.</param>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception captured.</param>
        public PageParseException(Uri page, string? message, Exception? innerException) : base(message, innerException)
        {
            Page = page;
        }

        /// <summary>
        /// The page Uri the problem occurred on.
        /// </summary>
        public Uri? Page { get; set; }
    }
}
