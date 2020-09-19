using AutoMapper;
using VoteMonitor.Api.Form.Queries;

namespace VoteMonitor.Api.Form.Mappers
{
    public class FormMapping : Profile
    {
        public FormMapping()
        {
            _ = CreateMap<DeleteFormModel, DeleteFormCommand>()
                .ForMember(dest => dest.FormId, c => c.MapFrom(src => src.FormId))
               ;
        }
    }
}
