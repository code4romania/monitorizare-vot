using AutoMapper;
using System;

namespace VoteMonitor.Api.Observer.Models
{
    public class ObserverModel
    {
        public int Id { get; set; }
        public string Ngo { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public DateTime? DeviceRegisterDate { get; set; }
        public DateTime? LastSeen { get; set; }
        public int NumberOfNotes { get; set; }
        public int NumberOfPollingStations { get; set; }
    }

    public class ObserverModelProfile : Profile
    {
        public ObserverModelProfile()
        {
            CreateMap<Entities.Observer, ObserverModel>()
                .ForMember(dest => dest.Ngo, c => c.MapFrom(src => src.Ngo.Name))
                .ForMember(dest => dest.NumberOfNotes, c => c.MapFrom(src => src.Notes.Count))
                .ForMember(dest => dest.NumberOfPollingStations, c => c.MapFrom(src => src.PollingStationInfos.Count));
        }
    }
}
