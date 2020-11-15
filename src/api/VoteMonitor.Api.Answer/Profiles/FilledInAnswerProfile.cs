using System.Linq;
using AutoMapper;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Answer.Profiles
{
    public class FilledInAnswerProfile : Profile
    {
        public FilledInAnswerProfile()
        {
            CreateMap<Question, QuestionDto<FilledInAnswerDto>>()
                .ForMember(dest => dest.Answers, c => c.MapFrom(src => src.OptionsToQuestions))
                .ForMember(dest => dest.Id, c => c.MapFrom(src => src.Id))
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text))
                .ForMember(dest => dest.QuestionTypeId, c => c.MapFrom(src => src.QuestionType))
                .ForMember(dest => dest.QuestionId, c => c.MapFrom(src => src.Code))
                .ForMember(dest => dest.FormCode, c => c.MapFrom(src => src.Code))
                ;

            CreateMap<OptionToQuestion, FilledInAnswerDto>()
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Option.Text))
                .ForMember(dest => dest.IsFreeText, c => c.MapFrom(src => src.Option.IsFreeText))
                .ForMember(dest => dest.OptionId, c => c.MapFrom(src => src.Id))
                .ForMember(dest => dest.IsFlagged, c => c.MapFrom(src => src.Flagged))
                .ForMember(dest => dest.Value, c => c.MapFrom(src => src.Answers.First().Value));
        }
    }
}
