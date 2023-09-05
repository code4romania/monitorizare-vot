using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Options;
using VotingIrregularities.Domain.Seed.Services;

namespace VotingIrregularities.Domain.Seed;

public class Program
{
    private static IConfigurationRoot _configuration;
    private static ILogger<Program> _logger;

    public static void Main()
    {
        _configuration = ConfigurationHelper.GetConfiguration();

        IServiceCollection services = new ServiceCollection();

        ConfigureServices(services);

        IServiceProvider provider = services.BuildServiceProvider();
        _logger = provider.GetService<ILogger<Program>>()!;

        using (var serviceScope = provider.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var seeder = serviceScope.ServiceProvider.GetService<VotingContextSeeder>()!;

            _logger.LogDebug("Initializing data seeding...");
            var success = seeder.SeedData();

            if (success)
            {

                _logger.LogDebug("Data seeded.");
            }
            else
            {
                _logger.LogWarning("Problems occurred when migrating data. See logs.");
            }
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Seed options
        var seedOptions = new SeedOption();
        _configuration.GetSection(SeedOption.SectionKey).Bind(seedOptions);
        services.AddSingleton(seedOptions);

        // Configure hash service
        if (seedOptions.HashServiceType == HashServiceType.ClearText)
        {
            services.AddSingleton<IHashService, ClearTextService>();
        }

        if (seedOptions.HashServiceType == HashServiceType.Hash)
        {
            services.AddSingleton<IHashService>(new SHA256HashService(seedOptions.PasswordSalt));
        }

        // Logging
        services.AddLogging(configure =>
        {
            configure.AddConfiguration(_configuration.GetSection("Logging"));
            configure.AddConsole();
            configure.AddDebug();
        });

        // DB Context
        var connectionString = _configuration.GetConnectionString("DefaultConnection")!;
        services.AddDbContext<VoteMonitorContext>(options =>
        {
            options.UseNpgsql(connectionString,
                x => x.MigrationsAssembly("VotingIrregularities.Domain.Seed"));
        });

        services.AddTransient<VotingContextSeeder>();
    }
}
