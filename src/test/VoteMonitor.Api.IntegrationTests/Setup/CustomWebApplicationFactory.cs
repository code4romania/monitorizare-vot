using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.IntegrationTests.Setup
{
    public class CustomWebApplicationFactory
        : WebApplicationFactory<Startup>
    {
        private readonly string _database;
        private readonly SqliteConnection _connection;

        public CustomWebApplicationFactory()
        {
            var database = $"{Guid.NewGuid()}.db";
            _database = database;

            _connection = new SqliteConnection($"DataSource={database}");
            _connection.Open();
        }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(
                    new Dictionary<string, string>
                    {
                        ["SecretKey"] = "super-signing-secret"
                    });
            });

            builder.ConfigureServices(services =>
            {
                services
                    .AddEntityFrameworkSqlite()
                    .AddDbContext<VoteMonitorContext>(options =>
                    {
                        options.UseSqlite(_connection);
                        options.UseInternalServiceProvider(services.BuildServiceProvider());
                    });

                using var scope = services.BuildServiceProvider().CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<VoteMonitorContext>();
                db.Database.EnsureCreated();
                db.SeedData();
            });
        }

        protected override IWebHostBuilder CreateWebHostBuilder()
            => WebHost.CreateDefaultBuilder(null).UseStartup<Startup>();

        protected override IHostBuilder CreateHostBuilder() => null;

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _connection.Close();
            File.Delete(_database);
        }
    }
}