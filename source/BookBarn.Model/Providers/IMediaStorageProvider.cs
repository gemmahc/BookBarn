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
        public Task<Media> UpsertFrom(string id, Uri source);

        /// <summary>
        /// Creates a write token to write to the storage location for media with the matching id.
        /// </summary>
        /// <param name="id">The id of the media.</param>
        /// <param name="duration">How long the write token is valid for.</param>
        /// <returns>The write token.</returns>
        public Task<MediaStorageToken> CreateWriteToken(string id, TimeSpan duration);

        /// <summary>
        /// Deletes the media matching the specified id.
        /// </summary>
        /// <param name="id">The id of the media to remove.</param>
        /// <returns>Waitable.</returns>
        public Task Delete(string id);

        /// <summary>
        /// Returns the metadata object for stored media (if it exists)
        /// </summary>
        /// <param name="id">The id of the media object.</param>
        /// <returns>The media metadata if it exists in storage. Otherwise null.</returns>
        public Task<Media?> GetMetadata(string id);

    }
}
