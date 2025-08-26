namespace BookBarn.Model.Providers
{
    /// <summary>
    /// Provides an interfact for accessing genre information in the data store.
    /// </summary>
    public interface IGenreDataProvider
    {
        /// <summary>
        /// Gets all genres in the data store.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Genre>> GetGenres();

        /// <summary>
        /// Returns the list of genres matching the specified query.
        /// </summary>
        /// <param name="query">The genre query.</param>
        /// <returns>The list of matching genres.</returns>
        public Task<IEnumerable<Genre>> QueryGenres(GenreQuery query);
    }
}
