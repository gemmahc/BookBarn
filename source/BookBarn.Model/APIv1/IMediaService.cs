namespace BookBarn.Model.Api.v1
{
    public interface IMediaService
    {
        /// <summary>
        /// Gets the media representation in storage.
        /// </summary>
        /// <param name="id">The id of the resource.</param>
        /// <returns>The media metadata object.</returns>
        public Task<Media> Get(string id);

        /// <summary>
        /// Deletes the specified media from storage.
        /// </summary>
        /// <param name="id">The id of the media.</param>
        /// <returns>Waitable.</returns>
        public Task Delete(string id);

        /// <summary>
        /// Gets a token granting the ability to write to the specified media.
        /// </summary>
        /// <param name="id">The id of the media.</param>
        /// <returns>The write token.</returns>
        public Task<MediaStorageToken> GetWriteToken(string id);
    }
}
