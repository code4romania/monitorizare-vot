using AutoMapper;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Profiles
{
    public class OptionToQuestionProfile : Profile
    {
        public OptionToQuestionProfile()
        {
            CreateMap<OptionToQuestionDTO, OptionToQuestion>()
                .ForMember(dest => dest.Option, c => c.MapFrom(src =>
                    new Option
                    {
                        Text = src.Text,
                        IsFreeText = src.IsFreeText
                    }))
                .ForMember(dest => dest.Id, c => c.Ignore())
                .ForMember(dest => dest.IdOption, c => c.Ignore())
                .ForMember(dest => dest.IdQuestion, c => c.Ignore());

            CreateMap<OptionToQuestion, OptionToQuestionDTO>()
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Option.Text))
                .ForMember(dest => dest.IsFreeText, c => c.MapFrom(src => src.Option.IsFreeText))
                .ForMember(dest => dest.OrderNumber, c => c.MapFrom(src => src.Option.OrderNumber))
                .ForMember(dest => dest.IdOption, c => c.MapFrom(src => src.Id));
        }
    }
}
