using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;
using Serilog;
using VotingIrregularities.Api.Extensions;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore;
using SimpleInjector.Integration.AspNetCore.Mvc;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.Models;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Redis;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VotingIrregularities.Api.Models.AccountViewModels;
using VotingIrregularities.Api.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using SimpleInjector.Lifestyles;
using Swashbuckle.AspNetCore.Swagger;

namespace VotingIrregularities.Api
{
    public class Startup
    {
        private readonly Container _container = new Container() { Options = { DefaultLifestyle = Lifestyle.Scoped, DefaultScopedLifestyle = new AsyncScopedLifestyle() } };
        private SymmetricSecurityKey _key;

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

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }


            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Get options from app settings
            services.AddOptions();
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["SecretKey"]));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            });

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                     {
                         options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                         options.RequireHttpsMetadata = false;
                         options.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                     });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("AppUser",
                                  policy => policy.RequireClaim("Organizatie", "Ong"));
            });

            services.AddApplicationInsightsTelemetry(Configuration);
            
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            //services.AddSwaggerGen();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Monitorizare Vot - API privat",
                    Description = "API care ofera suport aplicatiilor folosite de observatori.",
                    TermsOfService = "TBD",
                    Contact =
                        new Contact
                        {
                            Email = "info@monitorizarevot.ro",
                            Name = "Code for Romania",
                            Url = "http://monitorizarevot.ro"
                        },
                });

                options.OperationFilter<AddFileUploadParams>();

                var path = PlatformServices.Default.Application.ApplicationBasePath +
                           System.IO.Path.DirectorySeparatorChar + "VotingIrregularities.Api.xml";

                if (System.IO.File.Exists(path))
                    options.IncludeXmlComments(path);
            });

            services.UseSimpleInjectorAspNetRequestScoping(_container);

            ConfigureContainer(services);

            ConfigureCache(services);

            services.Configure<BlobStorageOptions>(Configuration.GetSection("BlobStorageOptions"));
            services.Configure<HashOptions>(Configuration.GetSection("HashOptions"));

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime, IDistributedCache cache)
        {
            app.UseStaticFiles();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            loggerFactory.AddSerilog();
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .ApplicationInsightsTraces(Configuration["ApplicationInsights:InstrumentationKey"])
                .CreateLogger();
            // app.UseApplicationInsightsRequestTelemetry();

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

            RegisterServices(app);

            ConfigureAzureStorage(app);

            ConfigureHash(app);

            InitializeContainer(app);

            RegisterDbContext<VotingContext>(Configuration.GetConnectionString("DefaultConnection"));

            RegisterAutomapper();

            BuildMediator();

            _container.Verify();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUI(o => o.SwaggerEndpoint("/swagger/v1/swagger.json", "MV API v1"));

            app.UseMvc();
        }

        private void ConfigureCache(IServiceCollection services)
        {
            var enableCache = Configuration.GetValue<bool>("ApplicationCacheOptions:Enabled");

            if (!enableCache)
            {
                _container.RegisterSingleton<ICacheService>(new NoCacheService());
                return;
            }

            var cacheProvider = Configuration.GetValue<string>("ApplicationCacheOptions:Implementation");


            _container.RegisterSingleton<ICacheService, CacheService>();

            switch (cacheProvider)
            {
                case "RedisCache":
                    {

                        services.AddDistributedRedisCache(options =>
                        {
                            Configuration.GetSection("RedisCacheOptions").Bind(options);
                        });

                        break;
                    }

                default:
                case "MemoryDistributedCache":
                    {

                        services.AddDistributedMemoryCache();
                        break;
                    }
            }
        }

        private void ConfigureAzureStorage(IApplicationBuilder app)
        {
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<BlobStorageOptions>>());
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptionsSnapshot<BlobStorageOptions>>());
            _container.RegisterSingleton<IFileService, BlobService>();
        }

        private void ConfigureHash(IApplicationBuilder app)
        {
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<HashOptions>>());
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
            _container.Register<ISectieDeVotareService, SectieDevotareDBService>(Lifestyle.Scoped);
            _container.RegisterSingleton(() => app.ApplicationServices.GetService<IOptions<JwtIssuerOptions>>());
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Add application presentation components:
            _container.RegisterMvcControllers(app);
            _container.RegisterMvcViewComponents(app);

            // Add application services. For instance:
            //container.Register<IUserRepository, SqlUserRepository>(Lifestyle.Scoped);


            // Cross-wire ASP.NET services (if any). For instance:
            _container.RegisterSingleton(app.ApplicationServices.GetService<ILoggerFactory>());
            _container.RegisterConditional(
                typeof(ILogger),
                c => typeof(Logger<>).MakeGenericType(c.Consumer.ImplementationType),
                Lifestyle.Singleton,
                c => true);

            // NOTE: Prevent cross-wired instances as much as possible.
            // See: https://simpleinjector.org/blog/2016/07/

            _container.RegisterSingleton<IConfigurationRoot>(Configuration);
        }

        private void RegisterDbContext<TDbContext>(string connectionString = null)
            where TDbContext : DbContext
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                var optionsBuilder = new DbContextOptionsBuilder<TDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                _container.RegisterSingleton(optionsBuilder.Options);

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
            _container.Register(typeof(IAsyncRequestHandler<,>), assemblies);
            _container.RegisterCollection(typeof(INotificationHandler<>), assemblies);
            _container.RegisterCollection(typeof(IAsyncNotificationHandler<>), assemblies);
            _container.RegisterSingleton(Console.Out);
            _container.RegisterSingleton(new SingleInstanceFactory(_container.GetInstance));
            _container.RegisterSingleton(new MultiInstanceFactory(_container.GetAllInstances));

            var mediator = _container.GetInstance<IMediator>();

            return mediator;
        }

        private void RegisterAutomapper()
        {
            Mapper.Initialize(cfg => { cfg.AddProfiles(GetAssemblies()); });

            _container.RegisterSingleton(Mapper.Configuration);
            _container.Register<IMapper>(() => new Mapper(Mapper.Configuration), Lifestyle.Scoped);
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(IMediator).GetTypeInfo().Assembly;
            yield return typeof(Startup).GetTypeInfo().Assembly;
            yield return typeof(VotingContext).GetTypeInfo().Assembly;
            // just to identify VotingIrregularities.Domain assembly
        }
    }
}