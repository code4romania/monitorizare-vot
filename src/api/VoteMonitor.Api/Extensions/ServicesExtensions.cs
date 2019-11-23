using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Core.Services;

namespace VoteMonitor.Api.Extensions
{
    public static class ServicesExtensions
    {

        public static IServiceCollection AddHashService(this IServiceCollection services, IConfiguration configuration)
        {
            var hashOptions = configuration.GetHashOptions().Get<HashOptions>();

            if (hashOptions.ServiceType == nameof(HashServiceType.ClearText))
                services.AddSingleton<IHashService, ClearTextService>();
            else
                services.AddSingleton<IHashService, HashService>();

            return services;
        }
    }
}
