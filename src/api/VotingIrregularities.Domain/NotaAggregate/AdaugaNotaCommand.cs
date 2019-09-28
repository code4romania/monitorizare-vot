using AutoMapper;
using MediatR;
using System;
using VoteMonitor.Entities;

namespace VotingIrregularities.Domain.NotaAggregate
{
    [Obsolete("Use AddNoteCommand instead ")]
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
        //public NotaProfile()
        //{
        //    CreateMap<AdaugaNotaCommand, Note>()
        //        .ForMember(dest => dest.IdQuestion, c => c.MapFrom(src =>
        //            !src.IdIntrebare.HasValue || src.IdIntrebare.Value <= 0 ? null : src.IdIntrebare)
        //        )
        //        .ForMember(dest => dest.LastModified, c => c.MapFrom(src => DateTime.UtcNow))
        //        .ForMember(dest => dest.AttachementPath, c => c.MapFrom(src => src.CaleFisierAtasat))
        //        .ForMember(dest => dest.IdObserver, c => c.MapFrom(src => src.IdObservator))
        //        .ForMember(dest => dest.IdPollingStation, c => c.MapFrom(src => src.IdSectieDeVotare))
        //        .ForMember(dest => dest.Text, c => c.MapFrom(src => src.TextNota));
        //}
    }
}
