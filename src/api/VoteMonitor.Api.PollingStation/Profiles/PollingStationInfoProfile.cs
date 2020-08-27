using AutoMapper;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Profiles
{
    public class PollingStationInfoProfile : Profile
    {
        public PollingStationInfoProfile()
        {
            CreateMap<Models.CreatePollingStationInfo, Queries.CreatePollingStationInfo>();
            CreateMap<CreatePollingStationInfo, Entities.PollingStationInfo>()
               .ForMember(dest => dest.LastModified, act => act.Ignore())
                .ForMember(dest => dest.Observer, act => act.Ignore())
                 .ForMember(dest => dest.PollingStation, act => act.Ignore());

            CreateMap<Models.EditPollingStationInfo, Queries.UpdatePollingStationInfo>();
            CreateMap<UpdatePollingStationInfo, Entities.PollingStationInfo>()
                .ForMember(dest=> dest.IdPollingStation, act=> act.Ignore())
                .ForMember(dest=> dest.IdObserver, act => act.Ignore());
        }
    }
}
