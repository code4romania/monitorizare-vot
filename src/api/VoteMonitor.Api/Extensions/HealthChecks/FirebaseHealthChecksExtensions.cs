using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace VoteMonitor.Api.Extensions.HealthChecks;

public static class FirebaseHealthChecksExtensions
{
    public static IHealthChecksBuilder AddFirebase(this IHealthChecksBuilder builder, string name)
        => builder.Add(new HealthCheckRegistration(
            name,
            _ => new FirebaseHealthCheck(), null, null, null));
}

public class FirebaseHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.GetApplicationDefault(),
                });
            }

            return Task.FromResult(HealthCheckResult.Healthy());
        }
        catch (Exception ex)
        {
            return Task.FromResult(new HealthCheckResult(context.Registration.FailureStatus, exception: ex));
        }
    }
}
