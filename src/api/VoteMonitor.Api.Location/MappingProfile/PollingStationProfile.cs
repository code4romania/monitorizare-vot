using System;
using AutoMapper;
using MonitorizareVot.Api.Location.Models;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;

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

            CreateMap<PollingStationDTO, PollingStation>()
                    .ForMember(dest => dest.Address, c => c.MapFrom(source => source.Adresa))
                    .ForMember(dest => dest.Number, c => c.MapFrom(source => source.NrSV))
                    .ForMember(dest => dest.AdministrativeTerritoryCode, c => c.MapFrom(source => source.CodSiruta));
        }
    }
}
