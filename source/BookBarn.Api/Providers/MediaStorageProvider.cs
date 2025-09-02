using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using BookBarn.Api.ErrorHandling;
using BookBarn.Model;
using BookBarn.Model.Providers;
using System.Globalization;

namespace BookBarn.Api.Providers
{
    public class MediaStorageProvider : IMediaStorageProvider
    {
        private BlobContainerClient _containerClient;

        public MediaStorageProvider(string connectionString, string containerName)
        {
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
        }
        public async Task<Media> UpsertFrom(string id, Uri source)
        {
            Dictionary<string, string> metadata = new Dictionary<string, string>()
            {
                {"Source", source.ToString() },
                {"FileName", Path.GetFileName(source.AbsolutePath) },
                {"Extension", Path.GetExtension(source.AbsolutePath) }
            };

            var blobClient = _containerClient.GetBlobClient(id);
            BlobCopyFromUriOptions opts = new BlobCopyFromUriOptions()
            {
                Metadata = metadata
            };

            var blobCopyInfo = await blobClient.SyncCopyFromUriAsync(source, opts);

            if (blobCopyInfo.Value.CopyStatus == CopyStatus.Success)
            {
                var props = await blobClient.GetPropertiesAsync();

                Media result = new Media()
                {
                    Checksum = Convert.ToBase64String(props.Value.ContentHash),
                    ContentType = props.Value.ContentType,
                    Location = blobClient.Uri.ToString(),
                    Name = blobClient.Name
                };

                return result;
            }

            throw new DataException(DataError.MediaUploadFailed, $"Blob upload returned with incomplete status [{blobCopyInfo.Value.CopyStatus}]");
        }

        public Task<MediaStorageToken> CreateWriteToken(string id, TimeSpan duration)
        {
            BlobSasBuilder builder = new BlobSasBuilder(
                    BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Add | BlobSasPermissions.Create,
                    new DateTimeOffset(DateTime.UtcNow.Add(duration), TimeSpan.Zero));

            var blobClient = _containerClient.GetBlobClient(id);
            Uri sas = blobClient.GenerateSasUri(builder);

            MediaStorageToken token = new MediaStorageToken()
            {
                Id = id,
                StorageEndpoint = sas,
                Headers = new Dictionary<string, string> {
                    { "x-ms-date", DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture) },
                    { "x-ms-version", "2025-07-05" },
                    { "x-ms-blob-type", "BlockBlob" }
                }
            };
               
            return Task.FromResult(token);
        }

        public async Task Delete(string id)
        {
            var blobClient = _containerClient.GetBlobClient(id);
            await blobClient.DeleteIfExistsAsync();
        }

        public async Task<Media?> GetMetadata(string id)
        {
            BlobClient blobClient = _containerClient.GetBlobClient(id);
            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var props = await blobClient.GetPropertiesAsync();

            Media result = new Media()
            {
                Checksum = Convert.ToBase64String(props.Value.ContentHash),
                ContentType = props.Value.ContentType,
                Location = blobClient.Uri.ToString(),
                Name = blobClient.Name
            };

            return result;
        }
    }
}
