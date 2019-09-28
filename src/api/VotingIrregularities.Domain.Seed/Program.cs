using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.Seed
{
    public class Program
    {
        private static IConfigurationRoot _configuration;
        private static ILogger _logger;

        public static void Main(string[] args)
        {
            BuildConfiguration();

            IServiceCollection services = new ServiceCollection();

            ConfigureServices(services);

            IServiceProvider provider = services.BuildServiceProvider();
            _logger = provider.GetService<ILogger<Program>>();

            using (var serviceScope = provider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<VoteMonitorContext>();
                _logger.LogDebug("Initializing Database for VotingContext...");
                context.Database.Migrate();
                _logger.LogDebug("Database created");

                if (!args.Contains("-seed")) return;
                _logger.LogDebug("Initializing data seeding...");
                context.EnsureSeedData();
                _logger.LogDebug("Data seeded.");
            }
        }

        private static void BuildConfiguration()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
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
            var conn = _configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<VoteMonitorContext>(options => {
            options.UseSqlServer(conn,
                x => x.MigrationsAssembly("VoteMonitor.Entities"));
                });
        }
    }
}
