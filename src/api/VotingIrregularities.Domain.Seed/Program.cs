using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System.IO;
using VoteMonitor.Api.HashingService;
using VoteMonitor.Entities;
using VotingIrregularities.Domain.Seed.Commands;
using VotingIrregularities.Domain.Seed.IoC;

namespace VotingIrregularities.Domain.Seed
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = BuildConfiguration();
            var registrations = ConfigureServices(configuration);

            // Create a type registrar and register any dependencies.
            // A type registrar is an adapter for a DI framework.
            var registrar = new TypeRegistrar(registrations);

            var app = new CommandApp(registrar);
            app.Configure(config =>
            {
                config.AddCommand<ApplyMigrationsCommand>("migrate");
                config.AddCommand<SeedCommand>("seed");
                config.AddCommand<ListNgoCommand>("list-ngos");
                config.AddCommand<ListNgoAdmins>("list-admins");
                config.AddCommand<AddNgoCommand>("add-ngo");
                config.AddCommand<AddNgoAdminCommand>("add-admin");
            });

            app.Run(args);
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables()
                .Build();
        }

        private static IServiceCollection ConfigureServices(IConfiguration configuration)
        {
            var registrations = new ServiceCollection();

            // DB Context
            var conn = configuration.GetConnectionString("DefaultConnection");
            registrations.AddDbContext<VoteMonitorContext>(options =>
            {
                options.UseSqlServer(conn,
                    x => x.MigrationsAssembly("VotingIrregularities.Domain.Seed"));
            });

            // Hashing
            registrations.AddHashService(configuration);

            return registrations;
        }
    }
}
