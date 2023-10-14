using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System;
using System.Net;

namespace VoteMonitor.Api.Extensions;

/// <summary>
/// extensions for global exception handling
/// </summary>
public static class ExceptionHandlerExtensions
{
    /// <summary>
    /// registers the default global exception handler which will log the exceptions on the server and return a user-friendly json response to the client when unhandled exceptions occur.
    /// TIP: when using this exception handler, you may want to turn off the asp.net core exception middleware logging to avoid duplication like so:
    /// <code>
    /// "Logging": { "LogLevel": { "Default": "Warning", "Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware": "None" } }
    /// </code>
    /// </summary>
    public static IApplicationBuilder UseDefaultExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(errApp =>
        {
            errApp.Run(async ctx =>
            {
                var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();
                if (exHandlerFeature is not null)
                {

                    var http = exHandlerFeature.Endpoint?.DisplayName?.Split(" => ")[0];
                    var type = exHandlerFeature.Error.GetType().Name;
                    var error = exHandlerFeature.Error.Message;
                    var msg =
$@"================================= 
{http} 
TYPE: {type} 
REASON: {error} 
--------------------------------- 
{exHandlerFeature.Error.StackTrace}";

                    Log.Error("{@http}{@type}{@reason}{@exception}", http, type, error, exHandlerFeature.Error);

                    ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    ctx.Response.ContentType = "application/problem+json";
                    await ctx.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Title = "An error occurred",
                        Detail = error,
                        Type = type,
                        Status = (int)HttpStatusCode.BadRequest
                    });
                }
            });
        });

        return app;
    }
}
