using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Extensions;
using VoteMonitor.Api.Form;
using VoteMonitor.Api.Location.Services;
using VoteMonitor.Entities;
using Serilog;
using VoteMonitor.Api;

const string CorsPolicyName = "Permissive";

var builder = WebApplication.CreateBuilder(args);
builder.Host.AddLoggingConfiguration(builder.Configuration, builder.Environment);

builder.Services.AddControllers();
builder.Services.ConfigureCustomOptions(builder.Configuration);
builder.Services.AddHashService(builder.Configuration);
builder.Services.AddFileService(builder.Configuration, builder.Environment);
builder.Services.AddVoteMonitorAuthentication(builder.Configuration);

builder.Services.AddScoped<IPollingStationService, PollingStationService>();

builder.Services.AddDbContext<VoteMonitorContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddFirebase(builder.Configuration);
builder.Services.AddAutoMapper(ImportHelper.GetAssemblies());
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(ImportHelper.GetAssemblies().ToArray());
});
builder.Services.AddFormServices();

builder.Services.ConfigureSwagger();
builder.Services.AddCachingService(builder.Configuration);
builder.Services.AddHealthChecks(builder.Configuration);

builder.Services.AddCors(options => options.AddPolicy(CorsPolicyName, builder =>
{
    builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

var app = builder.Build();
app.UseDefaultExceptionHandler();
app.UseSerilogRequestLogging();

app.UseStaticFiles();
if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseSwaggerAndUi();
app.UseCors(CorsPolicyName);
app.MapControllers();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, result) => await HealthChecksConfiguration.WriteResponse(context, result, builder.Environment)
});


try
{
    Log.Information("Starting application");
    await app.RunAsync();
}
catch (Exception e)
{
    Log.Error(e, "The application failed to start correctly");
    throw;
}
finally
{
    Log.Information("Shutting down application");
    Log.CloseAndFlush();
}
