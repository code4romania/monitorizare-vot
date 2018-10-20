using System;
using AutoMapper;
using MediatR;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.SectieAggregate
{
    public class InregistreazaSectieCommand : IRequest<int>
    {
        public int IdObservator { get; set; }
        public string CodJudet { get; set; }
        public int NumarSectie { get; set; }
        public DateTime? OraSosirii { get; set; }
        public DateTime? OraPlecarii { get; set; }
        public bool? EsteZonaUrbana { get; set; }
        public bool? PresedinteBesvesteFemeie { get; set; }
    }

    public class RaspunsFormularProfile : Profile
    {
        public RaspunsFormularProfile()
        {
            CreateMap<InregistreazaSectieCommand, PollingStationInfo>()
                .ForMember(dest => dest.IdObserver, c => c.MapFrom(src => src.IdObservator))
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.UrbanArea, c => c.MapFrom(src => src.EsteZonaUrbana))
                .ForMember(dest => dest.ObserverArrivalTime, c => c.MapFrom(src => src.OraSosirii))
                .ForMember(dest => dest.ObserverLeaveTime, c => c.MapFrom(src => src.OraPlecarii))
                .ForMember(dest => dest.IsPollingStationPresidentFemale, c => c.MapFrom(src => src.PresedinteBesvesteFemeie));

            CreateMap<ActualizeazaSectieCommand, PollingStationInfo>()
                .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.ObserverLeaveTime, c => c.MapFrom(src => src.OraPlecarii));
        }
    }
}
