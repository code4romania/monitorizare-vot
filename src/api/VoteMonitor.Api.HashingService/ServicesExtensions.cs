

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("VoteMonitor.Api.Tests")]

namespace VoteMonitor.Api.HashingService
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddHashService(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(HashOptions.Section);
            services.Configure<HashOptions>(section);
            var hashOptions = section.Get<HashOptions>();

            if (hashOptions.ServiceType == HashServiceType.ClearText)
            {
                services.AddSingleton<IHashService, ClearTextService>();
            }
            else
            {
                services.AddSingleton<IHashService, HashService>();
            }

            return services;
        }
    }
}
