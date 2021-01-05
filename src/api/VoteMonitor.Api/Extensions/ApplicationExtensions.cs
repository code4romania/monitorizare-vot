using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace VoteMonitor.Api.Extensions
{
    public static class ApplicationExtensions
    {
        public static IApplicationBuilder UseAppLocalization(this IApplicationBuilder app)
        {
            var localizationOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            return app;
        }
    }
}
