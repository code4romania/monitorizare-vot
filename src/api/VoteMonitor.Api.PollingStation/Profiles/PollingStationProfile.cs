using AutoMapper;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Profiles
{
    public class PollingStationProfile : Profile
    {
        public PollingStationProfile()
        {
            CreateMap<Entities.PollingStation, GetPollingStationModel>();
            CreateMap<PollingStationsFilterModel, GetPollingStations>();
            CreateMap<Models.UpdatePollingStationModel, UpdatePollingStation>()
                .ForMember(dest => dest.PollingStationId, act => act.Ignore());           
        }
    }
}
