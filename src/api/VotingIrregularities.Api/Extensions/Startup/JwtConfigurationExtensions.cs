using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using VoteMonitor.Api.Core;
using VotingIrregularities.Api.Models.AccountViewModels;

namespace VotingIrregularities.Api.Extensions.Startup
{
    public static class JwtConfigurationExtensions
    {
        public static IServiceCollection ConfigureVoteMonitorAuthentication(this IServiceCollection services,
            IConfigurationRoot configuration)
        {
            var jwtAppSettingOptions = configuration.GetSection(nameof(JwtIssuerOptions));

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration["SecretKey"]));

            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,

                RequireExpirationTime = false,
                ValidateLifetime = false,

                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                        options.RequireHttpsMetadata = false;
                        options.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                        options.TokenValidationParameters = tokenValidationParameters;
                    });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("NgoAdmin", policy => policy.RequireClaim(ClaimsHelper.UserType, UserType.NgoAdmin.ToString()));
                options.AddPolicy("Observer", policy => policy.RequireClaim(ClaimsHelper.UserType, UserType.Observer.ToString()).RequireClaim(ClaimsHelper.ObserverIdProperty));
                options.AddPolicy("Organizer", policy => policy.RequireClaim(ClaimsHelper.Organizer, "1"));
            });

            return services;
        }
    }
}
