using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VotingIrregularities.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace VotingIrregularities.Domain
{
    public class Program
    {

        public IConfigurationRoot Configuration { get; }

        public static void Main(string[] args)
        {
            var configuration  = Startup.RegisterConfiguration();
            ILoggerFactory loggerFactory = new LoggerFactory();
            var logger = loggerFactory.CreateLogger("Ef Migrations");
            loggerFactory.AddConsole(configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            IServiceCollection services = new ServiceCollection();

            services.AddSingleton(loggerFactory);
            var conn = configuration.GetConnectionString("DefaultConnection");


            services.AddDbContext<VotingContext>(options => options.UseSqlServer(conn));

            IServiceProvider provider = services.BuildServiceProvider();

            logger.LogDebug($"Initialized Context with {conn}");


            using (var serviceScope = provider.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<VotingContext>();
                logger.LogDebug($"Initializing Database for VotingContext...");
                context.Database.EnsureCreated();
                logger.LogDebug($"Database created");

                if (!args.Contains("-seed")) return;

                logger.LogDebug($"Initializing data seeding...");
                context.EnsureSeedData();
                logger.LogDebug($"Data seeded for {conn}");

            }
        }
    }
}
