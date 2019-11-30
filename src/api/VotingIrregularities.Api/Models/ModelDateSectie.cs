using System;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using VotingIrregularities.Domain.SectieAggregate;

namespace VotingIrregularities.Api.Models
{
    [Obsolete("Use AddPollingStationInfo instead.")]
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
