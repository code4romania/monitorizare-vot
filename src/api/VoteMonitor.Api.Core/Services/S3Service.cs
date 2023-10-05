using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.Extensions.Options;
using VoteMonitor.Api.Core.Models;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Services;

public class S3Service : IFileService
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3StorageOptions _options;

    public S3Service(IAmazonS3 s3Client, IOptions<S3StorageOptions> options)
    {
        _s3Client = s3Client;
        _options = options.Value;
        EnsureBucketExistsAsync(_options.BucketName).GetAwaiter().GetResult();
    }

    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        if (!(await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, bucketName)))
        {
            var putBucketRequest = new PutBucketRequest
            {
                BucketName = bucketName,
                UseClientRegion = true
            };

            _ = await _s3Client.PutBucketAsync(putBucketRequest);
        }
    }

    public async Task<UploadedFileModel> UploadFromStreamAsync(Stream sourceStream, string contentType, string extension, UploadType uploadType)
    {
        var fileKey = Guid.NewGuid().ToString("N") + extension;

        var request = new PutObjectRequest
        {
            BucketName = _options.BucketName,
            Key = fileKey,
            InputStream = sourceStream,
            ContentType = contentType
        };

        await _s3Client.PutObjectAsync(request);

        return CreatePreSignedUrl(fileKey);
    }

    public Task<string> GetPreSignedUrl(string filename)
    {
        return Task.FromResult(CreatePreSignedUrl(filename).Path);
    }

    private UploadedFileModel CreatePreSignedUrl(string fileKey)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _options.BucketName,
            Key = fileKey,
            Expires = DateTime.UtcNow.AddMinutes(_options.PresignedUrlExpirationInMinutes)
        };

        return new UploadedFileModel { FileName = fileKey, Path = _s3Client.GetPreSignedURL(request) };
    }
}
