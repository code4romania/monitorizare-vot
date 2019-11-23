using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VoteMonitor.Api.Core.Extensions;
using VoteMonitor.Api.Extensions;
using VoteMonitor.Entities;

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
            services.ConfigureCustomOptions(Configuration);
            services.AddHashService(Configuration);
            services.AddVoteMonitorAuthentication(Configuration);

            services.AddDbContext<VoteMonitorContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddMediatR(
                Assembly.GetAssembly(typeof(VoteMonitor.Api.Auth.Controllers.Authorization))
                //,Assembly.GetAssembly(typeof(VoteMonitor.Api.Answer.Controllers.AnswersController))
                );
            services.ConfigureSwagger();
            services.AddApplicationInsightsTelemetry();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSwaggerAndUi();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
