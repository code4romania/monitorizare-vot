using AutoMapper;
using System.ComponentModel.DataAnnotations;
using VotingIrregularities.Domain.NotaAggregate;

namespace VotingIrregularities.Api.Models
{
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
