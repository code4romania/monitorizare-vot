using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Builder.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.PlatformAbstractions;
using VotingIrregularities.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Migrations;

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
                logger.LogDebug($"Initializing data seeding...");
                context.EnsureSeedData();
                logger.LogDebug($"Data seeded for {conn}");

            }
        }
    }
}
