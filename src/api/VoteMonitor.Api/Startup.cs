using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Extensions;
using VoteMonitor.Api.Location.Services;
using VoteMonitor.Entities;
using VoteMonitor.Api.Form;

namespace VoteMonitor.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddControllers().AddNewtonsoftJson();
            services.ConfigureCustomOptions(Configuration);
            services.AddHashService(Configuration);
            services.AddFileService(Configuration);
            services.AddVoteMonitorAuthentication(Configuration);

            services.AddScoped<IPollingStationService, PollingStationService>();
            services.AddScoped<IFileLoader, XlsxFileLoader>();

            services.AddDbContext<VoteMonitorContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddFirebase(Configuration);
            services.AddAutoMapper(GetAssemblies());
            services.AddMediatR(GetAssemblies().ToArray());
            services.AddFormServices();

            services.ConfigureSwagger();
            services.AddApplicationInsightsTelemetry();
            services.AddCachingService(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSwaggerAndUi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private IEnumerable<Assembly> GetAssemblies()
        {
            yield return Assembly.GetAssembly(typeof(Startup));

            yield return typeof(Answer.Controllers.AnswersController).GetTypeInfo().Assembly;
            yield return typeof(Auth.Controllers.AuthorizationV1Controller).GetTypeInfo().Assembly;
            yield return typeof(County.Controllers.CountyController).GetTypeInfo().Assembly;
            yield return typeof(DataExport.Controllers.DataExportController).GetTypeInfo().Assembly;
            yield return typeof(Form.Controllers.FormController).GetTypeInfo().Assembly;
            yield return typeof(Location.Controllers.PollingStationController).GetTypeInfo().Assembly;
            yield return typeof(Note.Controllers.NoteController).GetTypeInfo().Assembly;
            yield return typeof(Notification.Controllers.NotificationController).GetTypeInfo().Assembly;            
            yield return typeof(Observer.Controllers.ObserverController).GetTypeInfo().Assembly;
            yield return typeof(Statistics.Controllers.StatisticsController).GetTypeInfo().Assembly;
            yield return typeof(PollingStation.Controllers.PollingStationController).GetTypeInfo().Assembly;
            yield return typeof(PollingStation.Controllers.PollingStationInfoController).GetTypeInfo().Assembly;
            yield return typeof(Core.Handlers.UploadFileHandler).GetTypeInfo().Assembly;
            yield return typeof(Core.Handlers.UploadFileHandler).GetTypeInfo().Assembly;
            yield return typeof(Ngo.Controllers.NgoAdminController).GetTypeInfo().Assembly;

        }
    }
}
