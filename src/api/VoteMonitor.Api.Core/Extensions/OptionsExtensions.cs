using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoteMonitor.Api.Core.Options;

namespace VoteMonitor.Api.Core.Extensions
{
    public static class OptionsExtensions
    {
        /// <summary>
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<HashOptions>(configuration.GetHashOptions());
            services.Configure<MobileSecurityOptions>(configuration.GetSection(nameof(MobileSecurityOptions)));
            services.Configure<FirebaseServiceOptions>(configuration.GetSection(nameof(FirebaseServiceOptions)));
            services.Configure<DefaultNgoOptions>(configuration.GetSection(nameof(DefaultNgoOptions)));
            services.Configure<ApplicationCacheOptions>(configuration.GetSection(nameof(ApplicationCacheOptions)));
            services.Configure<PollingStationsOptions>(configuration.GetSection(nameof(PollingStationsOptions)));
            return services;
        }

        public static IConfigurationSection GetHashOptions(this IConfiguration configuration)
        {
            return configuration.GetSection(nameof(HashOptions));
        }
    }
}
