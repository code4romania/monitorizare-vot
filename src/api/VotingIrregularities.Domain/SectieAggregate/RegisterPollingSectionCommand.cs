using AutoMapper;
using MediatR;
using System;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.SectieAggregate {
    public class RegisterPollingStationCommand : IRequest<int>
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public string CountyCode { get; set; }
        public bool? UrbanArea { get; set; }
        public DateTime? ObserverLeaveTime { get; set; }
        public DateTime? ObserverArrivalTime { get; set; }
        public bool? IsPollingStationPresidentFemale { get; set; }
    }

    public class PollingStationProfile : Profile {
        public PollingStationProfile() {
            CreateMap<RegisterPollingStationCommand, PollingStationInfo>()
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow));

            CreateMap<UpdatePollingSectionCommand, PollingStationInfo>()
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow));
        }
    }
}
