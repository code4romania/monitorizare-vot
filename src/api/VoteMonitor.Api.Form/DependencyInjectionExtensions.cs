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
            services.AddScoped<IUpdateOrCreateEntityMapper<Entities.Form, FormDTO>, HierarchicalMapper<Entities.Form, FormDTO, FormSection, FormSectionDTO>>();
            services.AddScoped<IUpdateOrCreateEntityMapper<FormSection, FormSectionDTO>, HierarchicalMapper<FormSection, FormSectionDTO, Question, QuestionDTO>>();
            services.AddScoped<IUpdateOrCreateEntityMapper<Question, QuestionDTO>, HierarchicalMapper<Question, QuestionDTO, OptionToQuestion, OptionToQuestionDTO>>();
            services.AddScoped<IUpdateOrCreateEntityMapper<OptionToQuestion, OptionToQuestionDTO>, UpdateOrCreateEntityMapper<OptionToQuestion, OptionToQuestionDTO>>();
            return services;
        }
    }
}
