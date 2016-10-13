using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Models
{
    public class ModelSectiune
    {
        public string CodSectiune { get; set; }
        public string Descriere { get; set; }

        public List<ModelIntrebare> Intrebari { get; set; }
    }


    public class FormularProfile : Profile
    {
        public FormularProfile()
        {
            CreateMap<Intrebare, ModelIntrebare>()
                .ForMember(src => src.RaspunsuriDisponibile, c => c.MapFrom(dest => dest.RaspunsDisponibil));

            CreateMap<RaspunsDisponibil, ModelRaspunsDisponibil>()
                .ForMember(dest => dest.TextOptiune, c => c.MapFrom(src => src.IdOptiuneNavigation.TextOptiune))
                .ForMember(dest => dest.SeIntroduceText, c => c.MapFrom(src => src.IdOptiuneNavigation.SeIntroduceText))
                .ForMember(dest => dest.IdOptiune, c => c.MapFrom(src => src.IdRaspunsDisponibil));
        }
    }
}
