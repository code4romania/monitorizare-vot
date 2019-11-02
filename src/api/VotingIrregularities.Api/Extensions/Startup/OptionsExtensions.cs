using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoteMonitor.Api.Core.Options;
using VotingIrregularities.Api.Options;

namespace VotingIrregularities.Api.Extensions.Startup
{
    public static class OptionsExtensions
    {
        public static IServiceCollection ConfigureCustomOptions(this IServiceCollection services,
            IConfigurationRoot configuration)
        {
            services.Configure<BlobStorageOptions>(configuration.GetSection(nameof(BlobStorageOptions)));
            services.Configure<HashOptions>(configuration.GetSection(nameof(HashOptions)));
            services.Configure<MobileSecurityOptions>(configuration.GetSection(nameof(MobileSecurityOptions)));
            services.Configure<FileServiceOptions>(configuration.GetSection(nameof(FileServiceOptions)));
            services.Configure<FirebaseServiceOptions>(configuration.GetSection(nameof(FirebaseServiceOptions)));
            services.Configure<DefaultNgoOptions>(configuration.GetSection(nameof(DefaultNgoOptions)));
            return services;
        }

        public static IServiceCollection ConfigureFirebase(this IServiceCollection services,
            IConfigurationRoot configuration)
        {

            return services;
        }
    }
} 
