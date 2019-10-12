﻿using AutoMapper;
using System;
using VoteMonitor.Entities;
using static VoteMonitor.Entities.VoteMonitorContext;

namespace VoteMonitor.Api.Answer.Models {
    public class PollingStationInfosDTO {
        public DateTime LastModified { get; set; }
        public bool? UrbanArea { get; set; }
        public DateTime? ObserverLeaveTime { get; set; }
        public DateTime? ObserverArrivalTime { get; set; }
        public bool? IsPollingStationPresidentFemale { get; set; }
    }

    public class RaspunsFomularProfile : Profile {
        public RaspunsFomularProfile() {
            CreateMap<PollingStationInfo, PollingStationInfosDTO>()
                .ForMember(dest => dest.LastModified, o => o.MapFrom(src => src.LastModified))
                .ForMember(dest => dest.UrbanArea, o => o.MapFrom(src => src.UrbanArea))
                .ForMember(dest => dest.ObserverLeaveTime, o => o.MapFrom(src => src.ObserverLeaveTime))
                .ForMember(dest => dest.ObserverArrivalTime, o => o.MapFrom(src => src.ObserverArrivalTime))
                .ForMember(dest => dest.IsPollingStationPresidentFemale, o => o.MapFrom(src => src.IsPollingStationPresidentFemale))
                ;
            CreateMap<AnswerQueryInfo, AnswerQueryDTO>()
                .ForMember(dest => dest.IdObserver, o=>o.MapFrom(src=>src.IdObserver))
                .ForMember(dest => dest.IdPollingStation, o => o.MapFrom(src => src.IdPollingStation))
                .ForMember(dest => dest.ObserverName, o => o.MapFrom(src => src.ObserverName))
                .ForMember(dest => dest.PollingStationName, o => o.MapFrom(src => src.PollingStation))
                ;
        }
    }
}
