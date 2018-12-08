using System;
using AutoMapper;
using MediatR;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.SectionAggregate
{
    public class RegisterSectionCommand : IRequest<int>
    {
        public int ObserverId { get; set; }
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
            CreateMap<RegisterSectionCommand, FormAnswer>()
                .ForMember(src => src.LastChangeDate, c => c.MapFrom(src => DateTime.UtcNow));

            CreateMap<SectionUpdateCommand, FormAnswer>()
                .ForMember(src => src.LastChangeDate, c => c.MapFrom(src => DateTime.UtcNow))
                .ForMember(src => src.LeaveDate, c => c.MapFrom(src => src.LeaveTime));
        }
    }
}
