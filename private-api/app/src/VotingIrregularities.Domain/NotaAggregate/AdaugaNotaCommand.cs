using AutoMapper;
using MediatR;
using System;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Domain.NotaAggregate
{
    public class AdaugaNotaCommand : IRequest<int>
    {
        public int IdObservator { get; set; }
        public int IdSectieDeVotare { get; set; }
        public int? IdIntrebare { get; set; }
        public string TextNota { get; set; }
        public string CaleFisierAtasat { get; set; }
    }

    public class NotaProfile : Profile
    {
        public NotaProfile()
        {
            CreateMap<AdaugaNotaCommand, Nota>()
                .ForMember(src => src.IdIntrebare, c => c.MapFrom(src => 
                    !src.IdIntrebare.HasValue || src.IdIntrebare.Value <= 0 ? null : src.IdIntrebare)
                 )
                .ForMember(src => src.DataUltimeiModificari, c => c.MapFrom(src => DateTime.UtcNow));
        }
    }
}
