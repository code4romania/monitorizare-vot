using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoteMonitor.Api.Core.Options;
using VotingIrregularities.Api.Options;

namespace VotingIrregularities.Api.Extensions.Startup
{
    public static class OptionsExtensions
    {
        /// <summary>
        /// At this point this is (I guess) useles;
        /// We use the SimpleInjector's container and registering these services in the default container does not benefit us quite a lot..
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services,
            IConfigurationRoot configuration)
        {
            services.Configure<BlobStorageOptions>(configuration.GetSection(nameof(BlobStorageOptions)));
            services.Configure<HashOptions>(configuration.GetSection(nameof(HashOptions)));
            services.Configure<MobileSecurityOptions>(configuration.GetSection(nameof(MobileSecurityOptions)));
            services.Configure<FileServiceOptions>(configuration.GetSection(nameof(FileServiceOptions)));
            services.Configure<FirebaseServiceOptions>(configuration.GetSection(nameof(FirebaseServiceOptions)));
            services.Configure<DefaultNgoOptions>(configuration.GetSection(nameof(DefaultNgoOptions)));
            services.Configure<DefaultNgoOptions>(configuration.GetSection(nameof(ApplicationCacheOptions)));
            services.Configure<PollingStationsOptions>(configuration.GetSection(nameof(PollingStationsOptions)));
            return services;
        }
    }
}
