using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Exceptions;

namespace VoteMonitor.Api.Extensions;

public static class LoggingConfiguration
{
    public static void AddLoggingConfiguration(this IHostBuilder host, IWebHostEnvironment env)
    {
        var loggingLevelSwitch = new LoggingLevelSwitch();

        if (env.IsDevelopment())
            loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;

        if (env.IsProduction())
            loggingLevelSwitch.MinimumLevel = LogEventLevel.Warning;
        
        var logger = new LoggerConfiguration()
            .MinimumLevel.ControlledBy(loggingLevelSwitch)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .Enrich.WithProperty("EnvironmentName", env.EnvironmentName)
            .Enrich.WithProperty("ApplicationName", env.ApplicationName)
            .Enrich.WithExceptionDetails()
            .WriteTo.Console();

        Log.Logger = logger.CreateLogger();
        
        host.UseSerilog();
    }
}
