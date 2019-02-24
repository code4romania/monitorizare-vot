using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using VotingIrregularities.Domain.SectieAggregate;

namespace VotingIrregularities.Api.Models
{
    public class ModelActualizareDateSectie
    {
        [Required(AllowEmptyStrings = false)]
        public string CodJudet { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int NumarSectie { get; set; }

        [Required(AllowEmptyStrings = false)]
        public DateTime? OraPlecarii { get; set; }
    }

    public class ModelActualizareDateSectieProfile : Profile
    {
        public ModelActualizareDateSectieProfile()
        {
            CreateMap<ModelActualizareDateSectie, ModelSectieQuery>();
            CreateMap<ModelActualizareDateSectie, ActualizeazaSectieCommand>();
        }
    }
}
