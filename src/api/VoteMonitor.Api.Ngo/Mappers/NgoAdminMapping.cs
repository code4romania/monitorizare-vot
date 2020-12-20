
using AutoMapper;
using VoteMonitor.Api.Ngo.Models;

namespace VoteMonitor.Api.Ngo.Mappers
{
    public class NgoAdminMapping : Profile
    {
        public NgoAdminMapping()
        {
            CreateMap<Entities.NgoAdmin, NgoAdminModel>(MemberList.Source)
                .ForMember(ngo => ngo.Password, cfg => cfg.Ignore());
        }
    }
}