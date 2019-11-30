using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.NotaAggregate;

namespace VoteMonitor.Api.Form.Models
{
    [Obsolete("use NoteModel")]
    public class ModelNota
    {
        [Required(AllowEmptyStrings = false)]
        public string CodJudet { get; set; }
        [Required]
        public int NumarSectie { get; set; }
        public int? IdIntrebare { get; set; }
        public string TextNota { get; set; }
    }

    public class ModelNotaProfile : Profile
    {
        public ModelNotaProfile()
        {
            CreateMap<ModelNota, ModelSectieQuery>();
            CreateMap<ModelNota, AdaugaNotaCommand>();
        }
    }
}
