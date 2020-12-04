using AutoMapper;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Profiles
{
    public class PollingStationInfoProfile : Profile
    {
        public PollingStationInfoProfile()
        {
            CreateMap<CreatePollingStationInfoModel, CreatePollingStationInfo>();
            CreateMap<CreatePollingStationInfo, Entities.PollingStationInfo>()
               .ForMember(dest => dest.LastModified, act => act.Ignore())
                .ForMember(dest => dest.Observer, act => act.Ignore())
                 .ForMember(dest => dest.PollingStation, act => act.Ignore())
                 .ForMember(dest => dest.IdPollingStation, opt=> opt.MapFrom(src=> src.PollingStationId))
                 .ForMember(dest => dest.IdObserver, opt=> opt.MapFrom(src=> src.ObserverId));

            CreateMap<EditPollingStationInfo, UpdatePollingStationInfo>();
            CreateMap<UpdatePollingStationInfo, Entities.PollingStationInfo>()
                .ForMember(dest=> dest.IdPollingStation, act=> act.Ignore())
                .ForMember(dest=> dest.IdObserver, act => act.Ignore());
        }
    }
}
