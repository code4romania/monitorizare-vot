using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Respawn;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoteMonitor.Entities;
using Xunit;

namespace VoteMonitor.Api.IntegrationTests.Setup
{
    [CollectionDefinition("Endpoints tests")]
    public class DatabaseCollection : ICollectionFixture<CustomWebApplicationFactory>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }


    public class CustomWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        private readonly Checkpoint _checkpoint;

        public CustomWebApplicationFactory()
        {
            _checkpoint = new Checkpoint
            {
                WithReseed = true
            };
        }



        public async Task Respawn()
        {
            using var scope = Services.GetService<IServiceScopeFactory>().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();
            var con = db.Database.GetDbConnection();
            await con.OpenAsync();
            await _checkpoint.Reset(con);
            db.SeedData();
        }


        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder
                    .AddInMemoryCollection(
                        new Dictionary<string, string>
                        {
                            ["SecretKey"] = "super-signing-secret"
                        });
            });

            builder.ConfigureServices(services =>
            {
                var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
                services.AddDbContext<VoteMonitorContext>(o => o.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

                var scope = services.BuildServiceProvider().CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<VoteMonitorContext>();
                db.Database.EnsureCreated();
            });
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
            => WebHost.CreateDefaultBuilder(null).UseStartup<Startup>();

        protected override IHostBuilder CreateHostBuilder() => null;
    }
}
