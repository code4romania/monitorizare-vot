using System;
using System.Collections.Generic;
using System.Text;
using VoteMonitor.Api.Models;
using VoteMonitor.Entities;
using AutoMapper;

namespace VoteMonitor.Api.Form.Models {
    public class FormDTO {
        public int Id { get; set; }
        public string Code { get; set; }
        public int CurrentVersion { get; set; }
        public string Description { get; set; }
        public List<FormSectionDTO> FormSections { get; set; }
    }

    public class FormProfile : Profile {
        public FormProfile() {
            CreateMap<FormDTO, Entities.Form>()
                .ForMember(dest => dest.Code, c => c.MapFrom(src => src.Code))
                .ForMember(dest => dest.CurrentVersion, c => c.MapFrom(src => src.CurrentVersion))
                .ForMember(dest => dest.Description, c => c.MapFrom(src => src.Description))
                .ForMember(dest => dest.FormSections, 
                    c => c.MapFrom(src => src.FormSections));

            CreateMap<FormSectionDTO, Entities.FormSection>()
                .ForMember(dest => dest.Code, c => c.MapFrom(src => src.Code))
                .ForMember(dest => dest.Description, c => c.MapFrom(src => src.Description))
                .ForMember(dest => dest.Questions,
                    c => c.MapFrom(src => src.Questions));

            CreateMap<QuestionDTO, Entities.Question>()
                .ForMember(dest => dest.Code, c => c.MapFrom(src => src.Code))
                .ForMember(dest => dest.Text, c => c.MapFrom(src => src.Text))
                .ForMember(dest => dest.QuestionType, c => c.MapFrom(src => src.QuestionType))
                .ForMember(dest => dest.OptionsToQuestions,
                    c => c.MapFrom(src => src.OptionsToQuestions));

            CreateMap<OptionToQuestionDTO, Entities.OptionToQuestion>()
                .ForMember(dest => dest.Option, c => c.MapFrom(src => 
                    new Option { 
                        Text = src.Text, 
                        IsFreeText = src.IsFreeText
                    }))
                ;
        }
    }
}
