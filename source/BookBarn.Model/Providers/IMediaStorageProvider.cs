namespace BookBarn.Model.Providers
{
    /// <summary>
    /// Provides an interfact for accessing the media data store.
    /// </summary>
    public interface IMediaStorageProvider
    {
        /// <summary>
        /// Adds or updates a media record in BookBarn storage from a specified source.
        /// </summary>
        /// <param name="source">The source uri containing the media.</param>
        /// <returns>The media information.</returns>
        public Task<Media> UpsertFrom(Uri source);
    }
}
