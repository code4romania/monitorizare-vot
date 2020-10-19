using AutoMapper;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Profiles
{

    public class QuestionProfile : Profile
    {
        public QuestionProfile()
        {
            CreateMap<Question, QuestionDTO>()
                .ForMember(dest => dest.OptionsToQuestions, c => c.MapFrom(src => src.OptionsToQuestions));

            CreateMap<QuestionDTO, Question>()
                .ForMember(dest => dest.OptionsToQuestions, c => c.MapFrom(src => src.OptionsToQuestions));

            CreateMap<OptionToQuestion, OptionToQuestionDTO>()
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Option.Text))
                .ForMember(dest => dest.IsFreeText, c => c.MapFrom(src => src.Option.IsFreeText))
                .ForMember(dest => dest.OrderNumber, c => c.MapFrom(src => src.Option.OrderNumber))
                .ForMember(dest => dest.IdOption, c => c.MapFrom(src => src.Id));
        }
    }
}
