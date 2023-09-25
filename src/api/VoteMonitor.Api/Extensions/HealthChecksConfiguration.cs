using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VoteMonitor.Api.Extensions;

public static class HealthChecksConfiguration
{
    public static async Task WriteResponse(HttpContext context, HealthReport result, IWebHostEnvironment env)
    {
        context.Response.ContentType = "application/json; charset=utf-8";

        var options = new JsonSerializerOptions
        {               
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        using var stream = new MemoryStream();

        var healthResponse = new
        {
            status = result.Status.ToString(),
            totalDuration = result.TotalDuration.ToString(),
            entries = result.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                tags = e.Value.Tags,
                description = e.Value.Description,
                data = env.IsDevelopment() && e.Value.Data.Count > 0 ? e.Value.Data : null,
                exception = env.IsDevelopment() ? ExtractSerializableExceptionData(e.Value.Exception) : null
            }).ToList()
        };

        await JsonSerializer.SerializeAsync(stream, healthResponse, healthResponse.GetType(), options);
        var json = Encoding.UTF8.GetString(stream.ToArray());
            
        await context.Response.WriteAsync(json);

        static object ExtractSerializableExceptionData(Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            return new
            {
                type = exception.GetType().ToString(),
                message = exception.Message,
                stackTrace = exception.StackTrace,
                source = exception.Source,
                data = exception.Data.Count > 0 ? exception.Data : null,
                innerException = exception.InnerException != null ? ExtractSerializableExceptionData(exception.InnerException) : null
            };
        }
    }
}
