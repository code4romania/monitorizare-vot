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
                var containerClient = await new BlobServiceClient(_storageOptions.Value.ConnectionString)
                    .CreateBlobContainerAsync(_storageOptions.Value.ContainerName, cancellationToken: cancellationToken);

                await containerClient.Value.GetPropertiesAsync(new BlobRequestConditions(), cancellationToken: cancellationToken);

                return HealthCheckResult.Healthy();
            }
            catch (Exception ex)
            {
                return new HealthCheckResult(context.Registration.FailureStatus, exception: ex);
            }
        }
    }
}
