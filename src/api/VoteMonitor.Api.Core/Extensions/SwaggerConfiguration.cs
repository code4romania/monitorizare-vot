using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using VotingIrregularities.Api.Extensions;

namespace VoteMonitor.Api.Core.Extensions {
    public static class SwaggerConfiguration {
        public static IServiceCollection ConfigureSwagger(this IServiceCollection services) {
            return services.AddSwaggerGen(options => {
                options.SwaggerDoc("v1", new OpenApiInfo {
                    Version = "v1",
                    Title = "VoteMonitor ",
                    Description = "API specs for NGO Admin and Observer operations.",
                    TermsOfService = new Uri("https://github.com/code4romania/monitorizare-vot/blob/refactor/major-wip/LICENSE"),
                    Contact =
                        new OpenApiContact {
                            Email = "info@monitorizarevot.ro",
                            Name = "Code for Romania",
                            Url = new Uri("https://votemonitor.org")
                        },
                });

                options.AddSecurityDefinition("bearer", new OpenApiSecurityScheme {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference {Type = ReferenceType.SecurityScheme, Id = "bearer"}
                            },
                            new[] {"readAccess", "writeAccess"}

                        }
                    }
                );


                options.OperationFilter<AddFileUploadParams>();

                var baseDocPath = Directory.GetCurrentDirectory();

                foreach (var api in Directory.GetFiles(baseDocPath, "*.xml")) {
                    options.IncludeXmlComments(api);
                }
            });
        }
    }
}
