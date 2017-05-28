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
            CreateMap<InregistreazaSectieCommand, RaspunsFormular>()
                .ForMember(src => src.DataUltimeiModificari, c => c.MapFrom(src => DateTime.UtcNow));

            CreateMap<ActualizeazaSectieCommand, RaspunsFormular>()
                .ForMember(src => src.DataUltimeiModificari, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(src => src.OraPlecarii, c => c.MapFrom(src => src.OraPlecarii));
        }
    }
}
