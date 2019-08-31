using System;
using AutoMapper;
using VoteMonitor.Api.Location.Commands;
using VotingIrregularities.Domain.Models;

namespace VoteMonitor.Api.Location.MappingProfile
{

    public class PollingStationProfile : Profile
    {
        public PollingStationProfile()
        {
            CreateMap<PollingStationInfo, RegisterPollingStationCommand>();

            CreateMap<RegisterPollingStationCommand, PollingStationInfo>()
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdatePollingSectionCommand, PollingStationInfo>()
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow));
        }
    }
}
