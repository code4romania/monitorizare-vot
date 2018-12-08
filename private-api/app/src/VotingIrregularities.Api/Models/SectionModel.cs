using System.Collections.Generic;
using AutoMapper;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Models
{
    public class SectionModel
    {
        public string SectionCode { get; set; }
        public string Description { get; set; }

        public List<QuestionModel> Questions { get; set; }
    }

    public class FormProfile : Profile
    {
        public FormProfile()
        {
            CreateMap<Question, QuestionModel>()
                .ForMember(src => src.AvailableAnswers, c => c.MapFrom(dest => dest.AvailableAnswer));

            CreateMap<AvailableAnswer, AvailableAnswersModel>()
                .ForMember(dest => dest.TextOptiune, c => c.MapFrom(src => src.OptionNavigationId.TextOption))
                .ForMember(dest => dest.SeIntroduceText, c => c.MapFrom(src => src.OptionNavigationId.TextMustBeInserted))
                .ForMember(dest => dest.IdOptiune, c => c.MapFrom(src => src.AvailableAnswerId));
        }
    }
}
