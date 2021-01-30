using AutoMapper;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Answer.Profiles
{
    public class AnswerProfile : Profile
    {
        public AnswerProfile()
        {
            CreateMap<PollingStationInfo, PollingStationInfoDto>()
                .ForMember(dest => dest.LastModified, o => o.MapFrom(src => src.LastModified))
                .ForMember(dest => dest.UrbanArea, o => o.MapFrom(src => src.UrbanArea))
                .ForMember(dest => dest.ObserverLeaveTime, o => o.MapFrom(src => src.ObserverLeaveTime))
                .ForMember(dest => dest.ObserverArrivalTime, o => o.MapFrom(src => src.ObserverArrivalTime))
                .ForMember(dest => dest.IsPollingStationPresidentFemale, o => o.MapFrom(src => src.IsPollingStationPresidentFemale))
                ;
            CreateMap<VoteMonitorContext.AnswerQueryInfo, AnswerQueryDto>()
                .ForMember(dest => dest.ObserverId, o => o.MapFrom(src => src.IdObserver))
                .ForMember(dest => dest.ObserverPhoneNumber, o => o.MapFrom(src => src.ObserverPhoneNumber))
                .ForMember(dest => dest.PollingStationId, o => o.MapFrom(src => src.IdPollingStation))
                .ForMember(dest => dest.ObserverName, o => o.MapFrom(src => src.ObserverName))
                .ForMember(dest => dest.PollingStationName, o => o.MapFrom(src => src.PollingStation))
                ;
        }
    }
}
