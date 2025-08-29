namespace BookBarn.Model.API.v1
{
    /// <summary>
    /// Interface for interacting with Genres in the book barn.
    /// </summary>
    public interface IGenresController
    {
        /// <summary>
        /// Gets all genres represented in the book barn.
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Genre>> Get();

        /// <summary>
        /// Gets a specific genre by name.
        /// </summary>
        /// <param name="name">The name of the genre.</param>
        /// <returns>The requested genre.</returns>
        public Task<Genre> Get(string name);
    }
}
