using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Migrator;

public class Program
{
    private static IConfigurationRoot _configuration;
    private static ILogger _logger;

    public static void Main()
    {
        _configuration = ConfigurationHelper.GetConfiguration();

        IServiceCollection services = new ServiceCollection();

        ConfigureServices(services);

        IServiceProvider provider = services.BuildServiceProvider();
        _logger = provider.GetService<ILogger<Program>>()!;

        using (var serviceScope = provider.GetService<IServiceScopeFactory>()!.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<VoteMonitorContext>()!;
            _logger.LogDebug("Initializing Database for VotingContext...");
            context.Database.Migrate();
            _logger.LogDebug("Database migrated");
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
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
                x => x.MigrationsAssembly("VotingIrregularities.Domain.Migrator"));
        });
    }
}
