using AutoMapper;
using VoteMonitor.Api.Form.Models;

namespace VoteMonitor.Api.Form.Profiles;

public class FormProfile : Profile
{
    public FormProfile()
    {
        CreateMap<FormDTO, Entities.Form>()
            .ForMember(dest => dest.FormSections, c => c.Ignore())
            .ForMember(dest => dest.Id, c => c.Ignore());
    }
}