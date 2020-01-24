using AutoMapper;

namespace VoteMonitor.Api.PollingStation.Profiles
{
    public class PollingStationProfile: Profile
    {
        public PollingStationProfile()
        {
            CreateMap<Entities.PollingStation, Models.PollingStation>();
        }
    }
}
