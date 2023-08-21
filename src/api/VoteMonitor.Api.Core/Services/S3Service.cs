using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Services
{
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

        public async Task<string> UploadFromStreamAsync(Stream sourceStream, string mimeType, string extension, UploadType uploadType)
        {
            var fileKey = Guid.NewGuid().ToString("N") + extension;
            new FileExtensionContentTypeProvider().TryGetContentType(extension, out var contentType);
        
            var request = new PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                InputStream = sourceStream,
                ContentType = contentType
            };

            await _s3Client.PutObjectAsync(request);

            return GetPreSignedUrl(fileKey);
        }

        private string GetPreSignedUrl(string fileKey)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = _options.BucketName,
                Key = fileKey,
                Expires = DateTime.Now.AddMinutes(_options.PresignedUrlExpirationInMinutes)
            };

            return _s3Client.GetPreSignedURL(request);
        }
    }
}
