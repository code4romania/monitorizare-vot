using Microsoft.Extensions.DependencyInjection;
using VoteMonitor.Api.Form.Mappers;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddFormServices(this IServiceCollection services)
        {
            services.AddScoped<IEntityMapper<Entities.Form, FormDTO>, HierarchicalMapper<Entities.Form, FormDTO, FormSection, FormSectionDTO>>();
            services.AddScoped<IEntityMapper<FormSection, FormSectionDTO>, HierarchicalMapper<FormSection, FormSectionDTO, Question, QuestionDTO>>();
            services.AddScoped<IEntityMapper<Question, QuestionDTO>, HierarchicalMapper<Question, QuestionDTO, OptionToQuestion, OptionToQuestionDTO>>();
            services.AddScoped(typeof(IUpdateOrCreateEntityMapper<,>), typeof(UpdateOrCreateEntityMapper<,>));
            services.AddScoped<IEntityMapper<OptionToQuestion, OptionToQuestionDTO>, UpdateOrCreateEntityMapper <OptionToQuestion, OptionToQuestionDTO>> ();
            return services;
        }
    }
}
