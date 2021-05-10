using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VoteMonitor.Api.Extensions.HealthChecks
{
    public static class ConditionalHealthChecksExtensions
    {
        public static IHealthChecksBuilder CheckOnlyWhen(this IHealthChecksBuilder builder, string name, Func<bool> predicate)
        {
            builder.Services.Configure<HealthCheckServiceOptions>(options =>
            {
                var registration = options.Registrations.FirstOrDefault(c => c.Name == name);

                if (registration == null)
                {
                    throw new InvalidOperationException($"A health check registration named `{name}` is not found in the health registrations list, so its conditional check cannot be configured. The registration must be added before configuring the conditional predicate.");
                }

                var factory = registration.Factory;
                registration.Factory = sp => new ConditionalHealthCheck(
                       () => factory(sp),
                       predicate,
                       sp.GetService<ILogger<ConditionalHealthCheck>>()
                   );
            });

            return builder;
        }
    }

    public class ConditionalHealthCheck : IHealthCheck
    {
        private const string NotChecked = "NotChecked";
        private readonly Func<bool> _predicate;
        private readonly ILogger<ConditionalHealthCheck> _logger;

        public ConditionalHealthCheck(Func<IHealthCheck> healthCheckFactory,
            Func<bool> predicate,
            ILogger<ConditionalHealthCheck> logger)
        {
            HealthCheckFactory = healthCheckFactory;
            _predicate = predicate;
            _logger = logger;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            context.Registration.Tags.Remove(NotChecked);

            if (!_predicate())
            {
                _logger.LogDebug("Healthcheck `{0}` will not be executed as its checking condition is not met.", context.Registration.Name);

                context.Registration.Tags.Add(NotChecked);

                return new HealthCheckResult(HealthStatus.Healthy, $"Health check on `{context.Registration.Name}` will not be evaluated " +
                    $"as its checking condition is not met. This does not mean your dependency is healthy, " +
                    $"but the health check operation on this dependency is not configured to be executed yet.");
            }

            return await HealthCheckFactory().CheckHealthAsync(context, cancellationToken);
        }

        internal Func<IHealthCheck> HealthCheckFactory { get; set; }
    }
}
