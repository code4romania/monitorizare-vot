using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                .ForMember(src => src.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(src => src.IdObserver, c => c.MapFrom(src => src.IdObservator))
                .ForMember(src => src.ObserverArrivalTime, c => c.MapFrom(src => src.OraSosirii))
                .ForMember(src => src.ObserverLeaveTime, c => c.MapFrom(src => src.OraPlecarii))
                .ForMember(src => src.IsPollingStationPresidentFemale, c => c.MapFrom(src => src.PresedinteBesvesteFemeie))
                ;

            CreateMap<ActualizeazaSectieCommand, PollingStationInfo>()
                .ForMember(src => src.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(src => src.ObserverLeaveTime, c => c.MapFrom(src => src.OraPlecarii));
        }
    }
}
