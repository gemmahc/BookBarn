using BookBarn.Model;
using BookBarn.Model.Api.v1;

namespace BookBarn.Api.Client
{
    public class MediaClient : ApiClient, IMediaService
    {
        public MediaClient(Uri endpoint) : base(endpoint) { }

        public MediaClient(HttpClient client) : base(client) { }

        public async Task Delete(string id)
        {
            var escapedId = Uri.EscapeDataString(id);
            await DeleteAsync(escapedId);
        }

        public async Task<Media> Get(string id)
        {
            var escapedId = Uri.EscapeDataString(id);
            return await GetAsync<Media>(escapedId);
        }

        public async Task<MediaStorageToken> GetWriteToken(string id)
        {
            var escapedId = Uri.EscapeDataString(id);
            return await GetAsync<MediaStorageToken>($"GetWriteToken/{escapedId}");
        }

        protected override string GetRoute()
        {
            return "/api/v1/Media";
        }
    }
}
