using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Extensions.HealthChecks
{
    public static class AzureBlobStorageHealthChecksExtensions
    {
        public static IHealthChecksBuilder AddAzureBlobStorage(this IHealthChecksBuilder builder, string name)
            => builder.Add(new HealthCheckRegistration(
                   name,
                   sp => new AzureBlobStorageHealthCheck(sp.GetService<IOptionsSnapshot<BlobStorageOptions>>()), null, null, null));
    }

    public class AzureBlobStorageHealthCheck : IHealthCheck
    {
        private IOptions<BlobStorageOptions> _storageOptions;

        public AzureBlobStorageHealthCheck(IOptions<BlobStorageOptions> storageOptions)
        {
            _storageOptions = storageOptions;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var credentials = new StorageCredentials(_storageOptions.Value.AccountName, _storageOptions.Value.AccountKey);
                var blobClient = new CloudStorageAccount(credentials, _storageOptions.Value.UseHttps).CreateCloudBlobClient();

                var serviceProperties = await blobClient.GetServicePropertiesAsync(
                    new BlobRequestOptions(),
                    operationContext: null,
                    cancellationToken: cancellationToken);

                var container = blobClient.GetContainerReference(_storageOptions.Value.Container);
                await container.FetchAttributesAsync();

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
