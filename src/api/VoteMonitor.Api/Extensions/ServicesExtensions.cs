using Amazon;
using Amazon.S3;
using Microsoft.AspNetCore.StaticFiles;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Extensions.HealthChecks;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddHashService(this IServiceCollection services, IConfiguration configuration)
    {
        var hashOptions = configuration.GetHashOptions().Get<HashOptions>();

        if (hashOptions!.HashServiceType == HashServiceType.ClearText)
        {
            services.AddSingleton<IHashService, ClearTextService>();
        }
        else
        {
            services.AddSingleton<IHashService, SHA256HashService>();
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
            case ApplicationCacheImplementationType.NoCache:
                {
                    services.AddSingleton<ICacheService, NoCacheService>();
                    break;
                }
            case ApplicationCacheImplementationType.RedisCache:
                {
                    services.AddSingleton<ICacheService, CacheService>();
                    services.AddStackExchangeRedisCache(options =>
                    {
                        configuration.GetSection("RedisCacheOptions").Bind(options);
                    });

                    break;
                }
            case ApplicationCacheImplementationType.MemoryDistributedCache:
                {
                    services.AddSingleton<ICacheService, CacheService>();
                    services.AddDistributedMemoryCache();
                    break;
                }
        }

        return services;
    }

    public static IServiceCollection AddFileService(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment environment)
    {
        services.AddSingleton<FileExtensionContentTypeProvider>();
        var fileStorageType = configuration.GetValue<FileStorageType>("FileStorageType");

        if (fileStorageType == FileStorageType.LocalFileService)
        {
            services.Configure<LocalFileStorageOptions>(configuration.GetSection(nameof(LocalFileStorageOptions)));
            services.AddSingleton<IFileService, LocalFileService>();
        }
        if (fileStorageType == FileStorageType.BlobService)
        {
            services.Configure<BlobStorageOptions>(configuration.GetSection(nameof(BlobStorageOptions)));
            services.AddSingleton<IFileService, BlobService>();
        }
        if (fileStorageType == FileStorageType.S3Service)
        {
            services.AddSingleton<IAmazonS3>(_ =>
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.EnumerableAllRegions
                        .FirstOrDefault(r => r.DisplayName == configuration.GetValue<string>("AWS:Region"))
                };

                if (environment.IsDevelopment())
                {
                    //settings to use with localstack S3 service
                    var serviceUrl = configuration.GetValue<string>("AWS:ServiceURL");
                    if (!string.IsNullOrEmpty(serviceUrl))
                    {
                        config.ServiceURL = configuration.GetValue<string>("AWS:ServiceURL");
                    }
                    config.ForcePathStyle = true;

                    var awsAccessKeyId = configuration.GetValue<string>("AWS:AWS_ACCESS_KEY_ID");
                    var awsSecretAccessKey = configuration.GetValue<string>("AWS:AWS_SECRET_ACCESS_KEY");
                    return new AmazonS3Client(awsAccessKeyId, awsSecretAccessKey, config);
                }

                return new AmazonS3Client(config);
            });

            services.Configure<S3StorageOptions>(configuration.GetSection(nameof(S3StorageOptions)));

            services.AddSingleton<IFileService, S3Service>();
        }

        return services;
    }

    public static IServiceCollection AddFirebase(this IServiceCollection services,
        IConfiguration configuration)
    {
        var firebaseOptions = configuration.GetSection(nameof(FirebaseServiceOptions));
        var privateKey = firebaseOptions[nameof(FirebaseServiceOptions.ServerKey)]!;
        var privateKeyPath = firebaseOptions[nameof(FirebaseServiceOptions.ServerPath)]!;

        var directoryName = Path.GetDirectoryName(privateKeyPath);
        if (!string.IsNullOrWhiteSpace(directoryName) && !Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        File.WriteAllText(privateKeyPath, privateKey);

        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", Path.GetFullPath(privateKeyPath));

        services.AddSingleton<IFirebaseService, FirebaseService>();

        return services;
    }

    public static IServiceCollection AddHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var enableHealthChecks = configuration.GetValue<bool>("EnableHealthChecks");

        services.AddHealthChecks()
            .AddDbContextCheck<VoteMonitorContext>()
            .AddRedis(configuration["RedisCacheOptions:Configuration"], "Redis")
            .CheckOnlyWhen("Redis", () => enableHealthChecks && configuration["ApplicationCacheOptions:Implementation"] == "RedisCache")
            .AddAzureBlobStorage("AzureBlobStorage")
            .CheckOnlyWhen("AzureBlobStorage", () => enableHealthChecks && configuration["FileServiceOptions:Type"] == "BlobService")
            .AddFirebase("Firebase")
            .CheckOnlyWhen("Firebase", () => enableHealthChecks && !string.IsNullOrEmpty(configuration["FirebaseServiceOptions:ServerKey"]))
            .AddS3Storage("S3")
            .CheckOnlyWhen("S3", () => enableHealthChecks && configuration["FileServiceOptions:Type"] == "S3Service");

        return services;
    }
}
