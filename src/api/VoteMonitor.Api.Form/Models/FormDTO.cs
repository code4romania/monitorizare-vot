using AutoMapper;
using System.Collections.Generic;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Models
{
    public class FormDTO {
        public int Id { get; set; }
        public string Code { get; set; }
        public int CurrentVersion { get; set; }
        public string Description { get; set; }
        public List<FormSectionDTO> FormSections { get; set; }
        public bool Diaspora { get; set; }
        public int Order { get; set; }
    }

    public class FormProfile : Profile {
        public FormProfile()
        {
	        CreateMap<FormDTO, Entities.Form>()
		        .ForMember(dest => dest.FormSections, c => c.MapFrom(src => src.FormSections))
		        .ForMember(dest => dest.Draft, c => c.MapFrom(src => false));

            CreateMap<FormSectionDTO, FormSection>()
                .ForMember(dest => dest.Questions,
                    c => c.MapFrom(src => src.Questions));

            CreateMap<QuestionDTO, Question>()
                .ForMember(dest => dest.OptionsToQuestions,
                    c => c.MapFrom(src => src.OptionsToQuestions));

            CreateMap<OptionToQuestionDTO, OptionToQuestion>()
                .ForMember(dest => dest.Option, c => c.MapFrom(src => 
                    new Option { 
                        Text = src.Text, 
                        IsFreeText = src.IsFreeText
                    }))
                ;
        }
    }
}
