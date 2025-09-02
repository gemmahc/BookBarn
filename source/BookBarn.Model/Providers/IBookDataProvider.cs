namespace BookBarn.Model.Providers
{
    /// <summary>
    /// Interfact providing access to the book data store
    /// </summary>
    public interface IBookDataProvider
    {
        /// <summary>
        /// Gets all books in the data store.
        /// </summary>
        /// <param name="count">The maximum number of records to retrieve.</param>
        /// <param name="afterId">The id to start returning from (exclusive).</param>
        /// <returns>The list of book instances.</returns>
        public Task<IEnumerable<Book>> GetMany(int count, string? afterId);

        /// <summary>
        /// Gets a specific book by Id in the data store.
        /// </summary>
        /// <param name="id">The id to match.</param>
        /// <returns>The matching book.</returns>
        public Task<Book> Get(string id);

        /// <summary>
        /// Deletes a book from the data store matching the specified id.
        /// </summary>
        /// <param name="id">The Id of the book to delete.</param>
        /// <returns>True if the book was deleted.</returns>
        public Task<bool> Delete(string id);

        /// <summary>
        /// Adds or updates the book in the data store.
        /// </summary>
        /// <param name="book">The book data to add/update.</param>
        /// <returns>The book as updated in the data store.</returns>
        public Task<Book> Upsert(Book book);

        /// <summary>
        /// Queries the data store for books matching the specified query.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>Matching book records.</returns>
        public Task<IEnumerable<Book>> QueryBooks(BookQuery query);
    }
}
