using System.Collections.Generic;
using AutoMapper;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Models
{
    public class ModelSectiune
    {
        public string IdSectiune { get; set; }
        public string CodSectiune { get; set; }
        public string Descriere { get; set; }

        public List<ModelIntrebare> Intrebari { get; set; }
    }


    /// <inheritdoc />
    public class FormularProfile : Profile
    {
        public FormularProfile()
        {
            CreateMap<Question, ModelIntrebare>()
                .ForMember(dest => dest.RaspunsuriDisponibile, c => c.MapFrom(src => src.OptionsToQuestions))
                .ForMember(dest => dest.IdIntrebare, c => c.MapFrom(src => src.Id))
                .ForMember(dest => dest.TextIntrebare, c => c.MapFrom(src => src.Text))
                .ForMember(dest => dest.IdTipIntrebare, c => c.MapFrom(src => (int)src.QuestionType))
                .ForMember(dest => dest.CodIntrebare, c => c.MapFrom(src => src.Code));

            CreateMap<OptionToQuestion, ModelRaspunsDisponibil>()
                .ForMember(dest => dest.TextOptiune, c => c.MapFrom(src => src.Option.Text))
                .ForMember(dest => dest.SeIntroduceText, c => c.MapFrom(src => src.Option.IsFreeText))
                .ForMember(dest => dest.IdOptiune, c => c.MapFrom(src => src.Id));
        }
    }
}
