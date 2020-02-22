using AutoMapper;
using System;
using VoteMonitor.Api.PollingStation.Models;
using VoteMonitor.Api.PollingStation.Queries;

namespace VoteMonitor.Api.PollingStation.Profiles
{
    public class PollingStationProfile : Profile
    {
        public PollingStationProfile()
        {
            CreateMap<Entities.PollingStation, Models.GetPollingStation>();
            CreateMap<PollingStationsFilter, GetPollingStations>();
            CreateMap<Models.UpdatePollingStation, Queries.UpdatePollingStation>()
                .ForMember(dest => dest.Id, act => act.Ignore());           
        }
    }
}
