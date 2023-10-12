using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Grafana.Loki;

namespace VoteMonitor.Api.Extensions;

public static class LoggingConfiguration
{
    public static void AddLoggingConfiguration(this IHostBuilder host, IConfiguration configuration, IWebHostEnvironment env)
    {
        var loggingLevelSwitch = new LoggingLevelSwitch();

        if (env.IsDevelopment())
        {
            loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
        }

        if (env.IsProduction())
        {
            loggingLevelSwitch.MinimumLevel = LogEventLevel.Warning;
        }

        var lokiCredentials = GetLokiCredentials(configuration);

        var logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(loggingLevelSwitch)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("Environment", env.EnvironmentName)
            .Enrich.WithProperty("Application", env.ApplicationName)
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.GrafanaLoki(configuration["LokiConfig:Uri"], propertiesAsLabels: new[] { "Environment", "Application" });

        Log.Logger = logger.CreateLogger();

        host.UseSerilog();
    }

    private static LokiCredentials GetLokiCredentials(IConfiguration configuration)
    {
        var user = configuration["LokiConfig:User"];
        var password = configuration["LokiConfig:Password"];

        if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(password))
        {
            return new LokiCredentials() { Login = user, Password = password };
        }

        return new LokiCredentials();
    }
}
