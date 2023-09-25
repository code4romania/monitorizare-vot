using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Services;

/// <inheritdoc />
public class BlobService : IFileService
{
    private BlobContainerClient _blobContainerClient;
    private BlobStorageOptions _storageOptions;

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
    public async Task<UploadedFileModel> UploadFromStreamAsync(Stream sourceStream, string contentType, string extension, UploadType uploadType)
    {
        // Get a reference to a blob
        var fileName = Guid.NewGuid().ToString("N") + extension;
        BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(sourceStream);

        var blobUri = GetBlobURI(blobClient);
        return new UploadedFileModel() { FileName = fileName, Path = blobUri.ToString() };
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
