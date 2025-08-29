namespace BookBarn.Model.API.v1
{
    /// <summary>
    /// Interface for interacting with Books in the Book Barn.
    /// </summary>
    public interface IBooksController
    {
        /// <summary>
        /// Gets a specified number of books from the collection starting after the book Id specified.
        /// </summary>
        /// <returns>The list of books.</returns>
        public Task<IEnumerable<Book>> Get(int count, string? afterId);

        /// <summary>
        /// Gets a specific book from the book barn matching the id.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The requested book.</returns>
        public Task<Book> Get(string id);

        /// <summary>
        /// Adds a the book to the book barn. 
        /// </summary>
        /// <param name="book">The book to add.</param>
        /// <returns>The persisted book record.</returns>
        public Task<Book> Post(Book book);

        /// <summary>
        /// Adds/Updates a book in the book barn.
        /// </summary>
        /// <param name="id">The id of the book.</param>
        /// <param name="book">The book to update.</param>
        /// <returns>The persisted book record.</returns>
        public Task<Book> Put(string id, Book book);

        /// <summary>
        /// Deletes the book with the matchin id from the book barn.
        /// </summary>
        /// <param name="id">The book id.</param>
        /// <returns>Waitable.</returns>
        public Task Delete(string id);

        /// <summary>
        /// Queries the book barn with the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>The list of books matching the query parameters.</returns>
        public Task<IEnumerable<Book>> Query(BookQuery query);
    }
}
