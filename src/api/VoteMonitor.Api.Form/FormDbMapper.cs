using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class FormDbMapper
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public FormDbMapper(VoteMonitorContext context, IMapper mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }

        public void Map(ref Entities.Form form, FormDTO dto)
        {
            form.Code = dto.Code;
            form.CurrentVersion = dto.CurrentVersion;
            form.Description = dto.Description;
            form.FormSections = new List<FormSection>();

            foreach (var fs in dto.FormSections)
            {
                var formSection = MapFormSection(fs);
                form.FormSections.Add(formSection);
            }
        }

        FormSection MapFormSection(FormSectionDTO formSectionDto)
        {
            var formSection = new FormSection
            {
                Code = formSectionDto.Code,
                Description = formSectionDto.Description,
                Questions = new List<Question>()
            };
            foreach (var q in formSectionDto.Questions)
            {
                var question = MapQuestion(q);
                formSection.Questions.Add(question);
            }

            return formSection;
        }

        Question MapQuestion(QuestionDTO questionDto)
        {
            var question = new Question { QuestionType = questionDto.QuestionType, Hint = questionDto.Hint, Text = questionDto.Text };
            var optionsForQuestion = new List<OptionToQuestion>();
            foreach (var o in questionDto.OptionsToQuestions)
                if (o.IdOption > 0)
                {
                    var existingOption = _context.Options.FirstOrDefault(option => option.Id == o.IdOption);
                    optionsForQuestion.Add(new OptionToQuestion { Option = existingOption });
                }
                else
                {
                    optionsForQuestion.Add(_mapper.Map<OptionToQuestion>(o));
                }

            question.OptionsToQuestions = optionsForQuestion;
            return question;
        }
    }
}