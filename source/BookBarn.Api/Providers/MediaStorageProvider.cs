using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
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
        private UserDelegationKey? _userDelegationKey;
        private string _accountName;

        public MediaStorageProvider(string connectionString, string containerName)
        {
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
            _accountName = _containerClient.GetParentBlobServiceClient().AccountName;
        }

        public MediaStorageProvider(Uri blobContainerUri, TokenCredential credential)
        {
            _containerClient = new BlobContainerClient(blobContainerUri, credential);
            _containerClient.CreateIfNotExists(PublicAccessType.None);
            _accountName = _containerClient.GetParentBlobServiceClient().AccountName;
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

        public async Task<MediaStorageToken> CreateWriteToken(string id, TimeSpan duration)
        {
            if (_userDelegationKey == null || _userDelegationKey.SignedExpiresOn < DateTimeOffset.UtcNow.AddMinutes(5))
            {
                // Create a new deligation key valid from 1 minute ago for 1 day.
                _userDelegationKey = await _containerClient
                                                .GetParentBlobServiceClient()
                                                .GetUserDelegationKeyAsync(
                                                    DateTimeOffset.UtcNow.AddMinutes(-1), 
                                                    DateTimeOffset.UtcNow.AddDays(1));
            }

            BlobClient blobClient = _containerClient.GetBlobClient(id);

            BlobSasBuilder builder = new BlobSasBuilder()
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
            };

            builder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write | BlobSasPermissions.Add | BlobSasPermissions.Create);

            BlobUriBuilder uriBuilder = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = builder.ToSasQueryParameters(_userDelegationKey, _accountName)
            };

            MediaStorageToken token = new MediaStorageToken()
            {
                Id = id,
                StorageEndpoint = uriBuilder.ToUri(),
                Headers = new Dictionary<string, string> {
                    { "x-ms-date", DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture) },
                    { "x-ms-version", "2025-07-05" },
                    { "x-ms-blob-type", "BlockBlob" }
                }
            };

            return token;
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
