using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VotingIrregularities.Domain.SectieAggregate;

namespace VotingIrregularities.Api.Models
{
    public class ModelDateSectie
    {
        [Required(AllowEmptyStrings = false)]
        public string CodJudet { get; set; }

        [Required(AllowEmptyStrings = false)]
        public int NumarSectie { get; set; }

        public DateTime? OraSosirii { get; set; }
        public DateTime? OraPlecarii { get; set; }
        public bool? EsteZonaUrbana { get; set; }
        public bool? PresedinteBesvesteFemeie { get; set; }
    }



    public class ModelDateSectieProfile : Profile
    {
        public ModelDateSectieProfile()
        {
            CreateMap<ModelDateSectie, InregistreazaSectieCommand>();
        }
    }
}
