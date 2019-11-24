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
                Draft = false
            };

            foreach (var fs in message.Form.FormSections)
            {
                var formSection = new FormSection
                {
                    Code = fs.Code,
                    Description = fs.Description,
                    Questions = new List<Question>()
                };
                foreach (var q in fs.Questions)
                {
                    var question = new Question { QuestionType = q.QuestionType, Hint = q.Hint, Text = q.Text };
                    var optionsForQuestion = new List<OptionToQuestion>();
                    foreach (var o in q.OptionsToQuestions)
                    {
                        if (o.IdOption > 0)
                        {
                            var existingOption = _context.Options.FirstOrDefault(option => option.Id == o.IdOption);
                            optionsForQuestion.Add(new OptionToQuestion { Option = existingOption });
                        }
                        else
                        {
                            optionsForQuestion.Add(_mapper.Map<OptionToQuestion>(o));
                        }
                    }

                    question.OptionsToQuestions = optionsForQuestion;
                    formSection.Questions.Add(question);
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