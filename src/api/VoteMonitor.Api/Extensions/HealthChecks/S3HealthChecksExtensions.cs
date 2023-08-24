using Amazon.S3;
using Amazon.S3.Model;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

using System;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Extensions.HealthChecks
{
    public static class S3HealthChecksExtensions
    {
        public static IHealthChecksBuilder AddS3Storage(this IHealthChecksBuilder builder, string name)
            => builder.Add(new HealthCheckRegistration(
                   name,
                   sp => new S3HealthCheck(sp.GetService<IAmazonS3>(), sp.GetService<IOptionsSnapshot<S3StorageOptions>>()), null, null, null));
    }

    public class S3HealthCheck : IHealthCheck
    {
        private readonly IAmazonS3 _s3Client;
        private readonly S3StorageOptions _storageOptions;

        public S3HealthCheck(IAmazonS3 s3Client, IOptions<S3StorageOptions> storageOptions)
        {
            _s3Client = s3Client;
            _storageOptions = storageOptions.Value;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var listRequest = new ListObjectsRequest
                {
                    BucketName = _storageOptions.BucketName,
                    MaxKeys = 1 // List only one key in the bucket to minimize data retrieved since we only check if we configured S3 connection correctly.
                };
                var _ = await _s3Client.ListObjectsAsync(listRequest, cancellationToken).ConfigureAwait(false);
                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
