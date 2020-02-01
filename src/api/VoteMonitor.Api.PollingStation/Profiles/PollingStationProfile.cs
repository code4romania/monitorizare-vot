using AutoMapper;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Profiles
{
    public class PollingStationProfile: Profile
    {
        public PollingStationProfile()
        {
            CreateMap<Entities.PollingStation, Models.PollingStation>();
            CreateMap<PollingStationsFilter, GetPollingStations>();
        }
    }
}
