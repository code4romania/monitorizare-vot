using AutoMapper;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Api.Location.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.MappingProfile;

public class PollingStationProfile : Profile
{
    public PollingStationProfile()
    {
        CreateMap<PollingStationInfo, RegisterPollingStationCommand>();

        CreateMap<RegisterPollingStationCommand, PollingStationInfo>(MemberList.None)
            .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsPollingStationPresidentFemale,
                c => c.MapFrom(src => src.IsPollingStationPresidentFemale))
            .ForMember(dest => dest.ObserverArrivalTime, c => c.MapFrom(src => src.ObserverArrivalTime))
            .ForMember(dest => dest.ObserverLeaveTime, c => c.MapFrom(src => src.ObserverLeaveTime))
            .ForMember(dest => dest.UrbanArea, c => c.MapFrom(src => src.UrbanArea));

        CreateMap<UpdatePollingSectionCommand, PollingStationInfo>(MemberList.None)
            .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.ObserverLeaveTime, c => c.MapFrom(src => src.ObserverLeaveTime));


        CreateMap<AddPollingStationInfo, RegisterPollingStationCommand>()
            .ForMember(dest => dest.CountyCode, c => c.MapFrom(src => src.CountyCode))
            .ForMember(dest => dest.IdPollingStation, c => c.MapFrom(src => src.IdPollingStation))
            .ForMember(dest => dest.IsPollingStationPresidentFemale, c => c.MapFrom(src => src.IsPollingStationPresidentFemale))
            .ForMember(dest => dest.ObserverArrivalTime, c => c.MapFrom(src => src.ObserverArrivalTime))
            .ForMember(dest => dest.ObserverLeaveTime, c => c.MapFrom(src => src.ObserverLeaveTime))
            ;
    }
}