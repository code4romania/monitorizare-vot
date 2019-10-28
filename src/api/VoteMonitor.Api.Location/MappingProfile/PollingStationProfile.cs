using System;
using AutoMapper;
using MonitorizareVot.Api.Location.Models;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Api.Location.Models;
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

            CreateMap<AddPollingStationInfo, RegisterPollingStationCommand>()
                .ForMember(dest => dest.CountyCode, c => c.MapFrom(src => src.CountyCode))
                .ForMember(dest => dest.IdPollingStation, c => c.MapFrom(src => src.IdPollingStation))
                .ForMember(dest => dest.IsPollingStationPresidentFemale, c => c.MapFrom(src => src.IsPollingStationPresidentFemale))
                .ForMember(dest => dest.ObserverArrivalTime, c => c.MapFrom(src => src.ObserverArrivalTime))
                .ForMember(dest => dest.ObserverLeaveTime, c => c.MapFrom(src => src.ObserverLeaveTime))
                ;

            CreateMap<PollingStationDTO, PollingStation>()
                    .ForMember(dest => dest.Address, c => c.MapFrom(source => source.Adresa))
                    .ForMember(dest => dest.Number, c => c.MapFrom(source => source.NrSV))
                    .ForMember(dest => dest.AdministrativeTerritoryCode, c => c.MapFrom(source => source.CodSiruta));
        }
    }
}
