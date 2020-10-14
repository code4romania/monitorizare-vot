using AutoMapper;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class AddFormQueryHandler :
        IRequestHandler<AddFormQuery, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public AddFormQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<FormDTO> Handle(AddFormQuery message, CancellationToken cancellationToken)
        {
            var newForm = new Entities.Form
            {
                Code = message.Form.Code,
                CurrentVersion = message.Form.CurrentVersion,
                Description = message.Form.Description,
                FormSections = new List<FormSection>(),
                Diaspora = message.Form.Diaspora,
                Draft = message.Form.Draft,
                Order = message.Form.Order
            };

            foreach (var (form, formIndex) in message.Form.FormSections.Select((form, formIndex) => (form, formIndex + 1)))
            {
                var formSection = new FormSection
                {
                    Code = form.Code,
                    Description = form.Description,
                    OrderNumber = formIndex,
                    Questions = new List<Question>()
                };
                foreach (var (question, questionIndex) in form.Questions.Select((question, questionIndex) => (question, questionIndex + 1)))
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
                newForm.FormSections.Add(formSection);
            }

            _context.Forms.Add(newForm);

            await _context.SaveChangesAsync();
            message.Form.Id = newForm.Id;
            return message.Form;
        }
    }
}