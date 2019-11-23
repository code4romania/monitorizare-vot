using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace VotingIrregularities.Api.Extensions {
    public static class SwaggerConfiguration {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services) {
            return services.AddSwaggerGen(options =>
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
        }
    }
}
