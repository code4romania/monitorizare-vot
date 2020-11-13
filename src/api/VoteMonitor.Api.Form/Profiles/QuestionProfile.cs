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
                .ForMember(dest => dest.OptionsToQuestions, c => c.Ignore())
                .ForMember(dest => dest.Id, c => c.Ignore())
                .ForMember(dest => dest.IdSection, c => c.Ignore());
        }
    }
}
