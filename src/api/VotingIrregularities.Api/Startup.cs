using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;
using VotingIrregularities.Api.Extensions;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using VoteMonitor.Entities;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.Options;
using VotingIrregularities.Api.Models.AccountViewModels;
using Microsoft.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;
using Swashbuckle.AspNetCore.Swagger;
using VotingIrregularities.Api.Options;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using VoteMonitor.Api.Answer.Controllers;
using VoteMonitor.Api.Location.Controllers;
using VoteMonitor.Api.Location.Services;
using VoteMonitor.Api.Observer.Controllers;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Note.Controllers;
using VoteMonitor.Api.Form.Controllers;
using VoteMonitor.Api.Core;
using VoteMonitor.Api.Core.Handlers;
using VoteMonitor.Api.Notification.Controllers;
using System.IO;
using VoteMonitor.Api.Core.Options;
using VoteMonitor.Api.Location.Options;
using VoteMonitor.Api.Statistics.Controllers;
using VotingIrregularities.Api.Extensions.Startup;

namespace VotingIrregularities.Api
{
    public class Startup
    {
        private readonly Container _container = new Container { Options = { DefaultLifestyle = Lifestyle.Scoped, DefaultScopedLifestyle = new AsyncScopedLifestyle() } };

        //private IConfiguration _configuration;

        //public Startup(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //}
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

            if (env.EnvironmentName.EndsWith("Development", StringComparison.CurrentCultureIgnoreCase))
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
            builder.AddApplicationInsightsSettings(developerMode: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get options from app settings
            services.AddOptions();
            services.ConfigureCustomOptions(Configuration);
           
            services.ConfigureVoteMonitorAuthentication(Configuration);
            services.AddApplicationInsightsTelemetry(Configuration);
            services.AddMvc(config =>
                {
                    config.Filters.Add(new AuthorizeFilter( new AuthorizationPolicyBuilder()
                                                                        .RequireAuthenticatedUser()
                                                                        .RequireClaim(ClaimsHelper.IdNgo)
                                                                        .Build())); 
                })
                .AddApplicationPart(typeof(PollingStationController).Assembly)
                .AddApplicationPart(typeof(ObserverController).Assembly)
                .AddApplicationPart(typeof(NotificationController).Assembly)
                .AddApplicationPart(typeof(NoteController).Assembly)
                .AddApplicationPart(typeof(FormController).Assembly)
                .AddApplicationPart(typeof(AnswersController).Assembly)
                .AddApplicationPart(typeof(StatisticsController).Assembly)
                .AddControllersAsServices()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "VoteMonitor ",
                    Description = "API specs for NGO Admin and Observer operations.",
                    TermsOfService = "TBD",
                    Contact =
                        new Contact
                        {
                            Email = "info@monitorizarevot.ro",
                            Name = "Code for Romania",
                            Url = "http://monitorizarevot.ro"
                        },
                });

                options.AddSecurityDefinition("bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>{
                    { "bearer", new[] {"readAccess", "writeAccess" } } });

                options.OperationFilter<AddFileUploadParams>();

                var baseDocPath = PlatformServices.Default.Application.ApplicationBasePath;

                foreach (var api in Directory.GetFiles(baseDocPath, "*.xml"))
                {
                    options.IncludeXmlComments(api);
                }
            });
            services.UseSimpleInjectorAspNetRequestScoping(_container);
            ConfigureContainer(services);

            ConfigureCache(services);
            ConfigureFileLoader();

            services.AddCors(options => options.AddPolicy("Permissive", builder =>
            {
	            builder.AllowAnyOrigin()
		            .AllowAnyMethod()
		            .AllowAnyHeader();
            }));

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app,
            IApplicationLifetime appLifetime)
        {
            app.UseStaticFiles();

            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .ApplicationInsights(TelemetryConfiguration.CreateDefault(), TelemetryConverter.Traces)
                .CreateLogger();

            appLifetime.ApplicationStopped.Register(Log.CloseAndFlush);

            app.UseExceptionHandler(
                builder =>
                {
                    builder.Run(context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";
                        return Task.FromResult(0);
                    }
                    );
                }
            );


            app.UseAuthentication();

            RegisterOptionsInSimpleInjector(app);

            RegisterServices(app);

            ConfigureFileService(app);

            ConfigureHash(app);

            InitializeContainer(app);

            //Registering dbContext
            RegisterDbContext<VoteMonitorContext>(Configuration.GetConnectionString("DefaultConnection"));

            RegisterAutomapper();

            app.AddFirebase(Configuration, _container);

            BuildMediator();

            _container.Verify();

            // initialization of the fileservice
            var fileService = _container.GetInstance<IFileService>();
            fileService.Initialize();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "MV API v1"));
            app.UseCors("Permissive");
			app.UseMvc();
        }

        private void RegisterOptionsInSimpleInjector(IApplicationBuilder app)
        {
            // these were already registered in the default container so we're going to pick them up from there..
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<MobileSecurityOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<DefaultNgoOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<FileServiceOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<BlobStorageOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<HashOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<JwtIssuerOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<ApplicationCacheOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<PollingStationsOptions>>());
        }

        private void ConfigureCache(IServiceCollection services)
        {
            var cacheOptions = new ApplicationCacheOptions();
            Configuration.GetSection(nameof(ApplicationCacheOptions)).Bind(cacheOptions);

            switch (cacheOptions.Implementation)
            {
                case "NoCache":
                    {
                        _container.RegisterInstance<ICacheService>(new NoCacheService());
                        break;
                    }
                case "RedisCache":
                    {
                        _container.RegisterSingleton<ICacheService, CacheService>();
                        services.AddDistributedRedisCache(options =>
                        {
                            Configuration.GetSection("RedisCacheOptions").Bind(options);
                        });

                        break;
                    }
                case "MemoryDistributedCache":
                    {
                        _container.RegisterSingleton<ICacheService, CacheService>();
                        services.AddDistributedMemoryCache();
                        break;
                    }
            }
        }

        private void ConfigureFileLoader()
        {
            _container.RegisterSingleton<IFileLoader, XlsxFileLoader>();
            return;
        }

        private void ConfigureFileService(IApplicationBuilder app)
        {
            var fileServiceOptions = app.ApplicationServices.GetService<IOptions<FileServiceOptions>>().Value;

            if (fileServiceOptions.Type == "LocalFileService")
                _container.RegisterSingleton<IFileService, LocalFileService>();
            else
                _container.RegisterSingleton<IFileService, BlobService>();
        }

        private void ConfigureHash(IApplicationBuilder app)
        {
            var hashOptions = app.ApplicationServices.GetService<IOptions<HashOptions>>().Value;

            if (hashOptions.ServiceType == nameof(HashServiceType.ClearText))
                _container.RegisterSingleton<IHashService, ClearTextService>();
            else
                _container.RegisterSingleton<IHashService, HashService>();
        }

        private void ConfigureContainer(IServiceCollection services)
        {
            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(_container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(_container));
        }

        private void RegisterServices(IApplicationBuilder app)
        {
            _container.Register<IPollingStationService, PollingStationService>(Lifestyle.Scoped);
            
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            // Cross-wire ASP.NET services (if any). For instance:
            _container.RegisterInstance(app.ApplicationServices.GetService<ILoggerFactory>());
            _container.RegisterConditional(
                typeof(ILogger),
                c => typeof(Logger<>).MakeGenericType(c.Consumer.ImplementationType),
                Lifestyle.Singleton,
                c => true);

            // NOTE: Prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/

            _container.RegisterInstance<IConfigurationRoot>(Configuration);
        }

        private void RegisterDbContext<TDbContext>(string connectionString = null)
            where TDbContext : DbContext
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();

                optionsBuilder.UseSqlServer(connectionString);

                _container.RegisterInstance(optionsBuilder.Options);
                _container.Register<TDbContext>(Lifestyle.Scoped);
            }
            else
            {
                _container.Register<TDbContext>(Lifestyle.Scoped);
            }
        }

        private IMediator BuildMediator()
        {
            var assemblies = GetAssemblies().ToArray();
            _container.RegisterSingleton<IMediator, Mediator>();
            _container.Register(typeof(IRequestHandler<,>), assemblies);
            _container.Register(typeof(AsyncRequestHandler<,>), assemblies);
            _container.Collection.Register(typeof(INotificationHandler<>), assemblies);
            _container.Collection.Register(typeof(AsyncNotificationHandler<>), assemblies);

            // had to add this registration as we were getting the same behavior as described here: https://github.com/jbogard/MediatR/issues/155
            _container.Collection.Register(typeof(IPipelineBehavior<,>), Enumerable.Empty<Type>());

            _container.RegisterInstance(Console.Out);
            _container.RegisterInstance(new SingleInstanceFactory(_container.GetInstance));
            _container.RegisterInstance(new MultiInstanceFactory(_container.GetAllInstances));

            var mediator = _container.GetInstance<IMediator>();

            return mediator;
        }

        private void RegisterAutomapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfiles(GetAssemblies()); });

            _container.RegisterInstance(Mapper.Configuration);
            _container.Register<IMapper>(() => new Mapper(Mapper.Configuration), Lifestyle.Scoped);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(Startup).GetTypeInfo().Assembly;
            yield return typeof(VoteMonitorContext).GetTypeInfo().Assembly;
            yield return typeof(PollingStationController).GetTypeInfo().Assembly;
            yield return typeof(ObserverController).GetTypeInfo().Assembly;
            yield return typeof(NoteController).GetTypeInfo().Assembly;
            yield return typeof(FormController).GetTypeInfo().Assembly;
            yield return typeof(AnswersController).GetTypeInfo().Assembly;
            yield return typeof(UploadFileHandler).GetTypeInfo().Assembly;
            yield return typeof(NotificationController).GetTypeInfo().Assembly;
            yield return typeof(StatisticsController).GetTypeInfo().Assembly;
            // just to identify VotingIrregularities.Domain assembly
        }
    }
}