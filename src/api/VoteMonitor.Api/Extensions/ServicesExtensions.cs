using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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
            {
                services.AddSingleton<IHashService, ClearTextService>();
            }
            else
            {
                services.AddSingleton<IHashService, HashService>();
            }

            return services;
        }

        public static IServiceCollection AddCachingService(this IServiceCollection services,
            IConfiguration configuration)
        {
            var cacheOptions = new ApplicationCacheOptions();
            configuration.GetSection(nameof(ApplicationCacheOptions)).Bind(cacheOptions);

            switch (cacheOptions.Implementation)
            {
                case "NoCache":
                    {
                        services.AddSingleton<ICacheService, NoCacheService>();
                        break;
                    }
                case "RedisCache":
                    {
                        services.AddSingleton<ICacheService, CacheService>();
                        services.AddDistributedRedisCache(options =>
                        {
                            configuration.GetSection("RedisCacheOptions").Bind(options);
                        });

                        break;
                    }
                case "MemoryDistributedCache":
                    {
                        services.AddSingleton<ICacheService, CacheService>();
                        services.AddDistributedMemoryCache();
                        break;
                    }
            }

            return services;
        }

        public static IServiceCollection AddFileService(this IServiceCollection services,
            IConfiguration configuration)
        {
            var fileServiceOptions = new FileServiceOptions();
            configuration.GetSection(nameof(FileServiceOptions)).Bind(fileServiceOptions);

            if (fileServiceOptions.Type == "LocalFileService")
            {
                services.AddSingleton<IFileService, LocalFileService>();
            }
            else
            {
                services.AddSingleton<IFileService, BlobService>();
            }

            return services;
        }

        public static IServiceCollection AddFirebase(this IServiceCollection services,
            IConfiguration configuration)
        {
            var firebaseOptions = configuration.GetSection(nameof(FirebaseServiceOptions));
            var privateKeyPath = firebaseOptions[nameof(FirebaseServiceOptions.ServerKey)];
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.GetFullPath(privateKeyPath));

            services.AddSingleton<IFirebaseService, FirebaseService>();

            return services;
        }
    }
}
