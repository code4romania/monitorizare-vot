using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Services
{
    /// <inheritdoc />
    public class BlobService : IFileService
    {
        private BlobContainerClient _blobContainerClient;
        private BlobStorageOptions _storageOptions;

        /// <inheritdoc />
        public BlobService(IOptions<BlobStorageOptions> storageOptions)
        {
            _storageOptions = storageOptions.Value;
            _blobContainerClient = GetOrCreateContainerClient();
        }

        private BlobContainerClient GetOrCreateContainerClient()
        {
            var blobServiceClient = new BlobServiceClient(_storageOptions.ConnectionString);
            var blobContainerClient = blobServiceClient.GetBlobContainerClient(_storageOptions.ContainerName);

            if (!blobContainerClient.Exists())
            {
                blobServiceClient.CreateBlobContainer(_storageOptions.ContainerName);
            }

            return blobContainerClient;
        }

        /// <summary>
        /// Uploads a file from a stream in azure blob storage
        /// </summary>
        public async Task<string> UploadFromStreamAsync(Stream sourceStream, string mimeType, string extension, UploadType uploadType)
        {
            // Get a reference to a blob
            BlobClient blobClient = _blobContainerClient.GetBlobClient(Guid.NewGuid().ToString("N") + extension);
            await blobClient.UploadAsync(sourceStream);

            var blobUri = GetBlobURI(blobClient);
            return blobUri.ToString();
        }

        private Uri GetBlobURI(BlobClient blobClient)
        {
            // Check if BlobContainerClient object has been authorized with Shared Key
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for 30 days
                BlobSasBuilder sasBuilder = new BlobSasBuilder
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b",
                    ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(_storageOptions.SASBlobAvailabilityInMinutes)
                };

                sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                var sasURI = blobClient.GenerateSasUri(sasBuilder);

                return sasURI;
            }

            throw new ApplicationException("Cannot create SAS token for requested blob");
        }
    }
}
