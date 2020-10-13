using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using VoteMonitor.Api.Form.Models;
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
            form.Diaspora = dto.Diaspora;
            form.Draft = dto.Draft;
            form.Order = dto.Order;

            foreach (var (formSect, formSectIndex) in dto.FormSections.Select((formSect, formSectIndex) => (formSect, formSectIndex + 1)))
            {
                var formSection = new FormSection
                {
                    Code = formSect.Code,
                    Description = formSect.Description,
                    OrderNumber = formSectIndex,
                    Questions = new List<Question>()
                };
                foreach (var (question, questionIndex) in formSect.Questions.Select((question, questionIndex) => (question, questionIndex + 1)))
                {
                    var questionEntity = new Question
                    {
                        QuestionType = question.QuestionType,
                        Hint = question.Hint,
                        Text = question.Text,
                        Code = question.Code,
                        OrderNumber = questionIndex
                    };

                    var optionsForQuestion = new List<OptionToQuestion>();
                    foreach (var (option, optionIndex) in question.OptionsToQuestions.Select((option, optionIndex) => (option, optionIndex + 1)))
                    {
                        if (option.IdOption > 0)
                        {
                            var existingOption = _context.Options.FirstOrDefault(o => o.Id == option.IdOption);
                            existingOption.OrderNumber = optionIndex;
                            optionsForQuestion.Add(new OptionToQuestion
                            {
                                Option = existingOption,
                                Flagged = option.Flagged
                            });
                        }
                        else
                        {
                            OptionToQuestion newOptionToQuestion = _mapper.Map<OptionToQuestion>(option);
                            newOptionToQuestion.Option.OrderNumber = optionIndex;
                            optionsForQuestion.Add(newOptionToQuestion);
                        }
                    }

                    questionEntity.OptionsToQuestions = optionsForQuestion;
                    formSection.Questions.Add(questionEntity);
                }
                form.FormSections.Add(formSection);
            }
        }
    }
}