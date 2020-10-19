using Microsoft.Extensions.DependencyInjection;
using VoteMonitor.Api.Form.Mappers;

namespace VoteMonitor.Api.Form
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddFormServices(this IServiceCollection services)
        {
            services.AddScoped<IFormMapper, FormMapper>();
            services.AddScoped<IFormSectionMapper, FormSectionMapper>();
            services.AddScoped<IQuestionMapper, QuestionMapper>();
            return services;
        }
    }
}
