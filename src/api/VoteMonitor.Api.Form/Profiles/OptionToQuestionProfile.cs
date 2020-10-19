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
                    }));
        }
    }
}
