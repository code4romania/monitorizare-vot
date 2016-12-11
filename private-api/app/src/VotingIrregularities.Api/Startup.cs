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
using Swashbuckle.Swagger.Model;
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

namespace VotingIrregularities.Api
{
    public class Startup
    {
        private readonly Container _container = new Container();
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
                builder.AddUserSecrets();

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

            services.AddSwaggerGen();

            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info
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

            ConfigureContainer(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime, IDistributedCache cache)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            loggerFactory.AddSerilog();
            Log.Logger = new LoggerConfiguration()
                .WriteTo
                .ApplicationInsightsTraces(Configuration["ApplicationInsights:InstrumentationKey"])
                .CreateLogger();
            app.UseApplicationInsightsRequestTelemetry();
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
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,

                RequireExpirationTime = false,
                ValidateLifetime = false,

                ClockSkew = TimeSpan.Zero
            };

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });
            app.UseSimpleInjectorAspNetRequestScoping(_container);

            _container.Options.DefaultScopedLifestyle = new AspNetRequestLifestyle();

            ConfigureCache(env);

            ConfigureAzureStorage(env);

            RegisterServices(app);

            ConfigureHash();

            InitializeContainer(app);

            RegisterDbContext<VotingContext>(Configuration.GetConnectionString("DefaultConnection"));

            RegisterAutomapper();

            BuildMediator();


            _container.Verify();

            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();
        }

        private void ConfigureCache(IHostingEnvironment env)
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
                        _container.RegisterSingleton<IDistributedCache>(
                          new RedisCache(
                              new OptionsManager<RedisCacheOptions>(new List<IConfigureOptions<RedisCacheOptions>>
                              {
                                new ConfigureFromConfigurationOptions<RedisCacheOptions>(
                                    Configuration.GetSection("RedisCacheOptions"))
                               })
                          ));
                        break;
                    }

                default:
                case "MemoryDistributedCache":
                    {
                        _container.RegisterSingleton<IDistributedCache>(new MemoryDistributedCache(new MemoryCache(new MemoryCacheOptions())));
                        break;
                    }
            }
        }

        private void ConfigureAzureStorage(IHostingEnvironment env)
        {
            _container.RegisterSingleton<IOptions<BlobStorageOptions>>(
                new OptionsManager<BlobStorageOptions>(new List<IConfigureOptions<BlobStorageOptions>>
                {
                    new ConfigureFromConfigurationOptions<BlobStorageOptions>(
                        Configuration.GetSection("BlobStorageOptions"))
                }));

            _container.RegisterSingleton<IFileService, BlobService>();
        }

        private void ConfigureHash()
        {
            _container.RegisterSingleton<IOptions<HashOptions>>(new OptionsManager<HashOptions>(new List<IConfigureOptions<HashOptions>>
                {
                    new ConfigureFromConfigurationOptions<HashOptions>(
                        Configuration.GetSection("HashOptions"))
                }));
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
            _container.RegisterSingleton<IHashService, HashService>();
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