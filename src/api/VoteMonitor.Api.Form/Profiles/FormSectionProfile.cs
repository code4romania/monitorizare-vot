using AutoMapper;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Profiles
{
    public class FormSectionProfile : Profile
    {
        public FormSectionProfile()
        {
            CreateMap<FormSectionDTO, FormSection>()
                .ForMember(dest => dest.Questions, c => c.Ignore())
                .ForMember(dest => dest.Id, c => c.Ignore())
                .ForMember(dest => dest.IdForm, c => c.Ignore());
        }
    }
}
