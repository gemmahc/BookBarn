using BookBarn.Model;
using BookBarn.Model.Api.v1;
using BookBarn.Model.Providers;

namespace BookBarn.Api.Controllers
{
    public class MediaControllerCore : IMediaService
    {
        private IMediaStorageProvider _storageProvider;
        private ILogger _logger;

        public MediaControllerCore(IMediaStorageProvider storageProvider, ILogger logger)
        {
            _storageProvider = storageProvider;
            _logger = logger;
        }

        public async Task Delete(string id)
        {
            await _storageProvider.Delete(id);
        }

        public async Task<Media> Get(string id)
        {
            Media? result = await _storageProvider.GetMetadata(id);

            if (result == null)
            {
                throw new DataException(ErrorHandling.DataError.NotFound);
            }

            return result;
        }

        public async Task<MediaStorageToken> GetWriteToken(string id)
        {
            MediaStorageToken token = await _storageProvider.CreateWriteToken(id, TimeSpan.FromMinutes(5));

            if (token == null)
            {
                throw new DataException(ErrorHandling.DataError.General);
            }

            return token;
        }
    }
}
