
using AutoMapper;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Mappers
{
    public class NgoMapping : Profile
    {
        public NgoMapping()
        {
            CreateMap<CreateUpdateNgoModel, Entities.Ngo>()
                .ForMember(ngo => ngo.Organizer, cfg => cfg.MapFrom(src => src.Organizer))
                .ForMember(ngo => ngo.Name, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(ngo => ngo.ShortName, cfg => cfg.MapFrom(src => src.ShortName))
                .ForMember(ngo => ngo.IsActive, cfg => cfg.MapFrom(src => src.IsActive))
                .ForAllOtherMembers(a => a.Ignore());

            CreateMap<Entities.Ngo, NgoModel>(MemberList.Source);

            CreateMap<NgoModel, Entities.Ngo>()
                .ForMember(ngo => ngo.Organizer, cfg => cfg.MapFrom(src => src.Organizer))
                .ForMember(ngo => ngo.Name, cfg => cfg.MapFrom(src => src.Name))
                .ForMember(ngo => ngo.ShortName, cfg => cfg.MapFrom(src => src.ShortName))
                .ForMember(ngo => ngo.IsActive, cfg => cfg.MapFrom(src => src.IsActive))
                .ForMember(ngo => ngo.Id, cfg => cfg.MapFrom(src => src.Id))
                .ForAllOtherMembers(a => a.Ignore());
        }
    }
}