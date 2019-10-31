using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class UpdateFormQueryHandler :
        AsyncRequestHandler<UpdateFormQuery, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;

        public UpdateFormQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override async Task<FormDTO> HandleCore(UpdateFormQuery message)
        {
            var form = _context.Forms.Find(message.Id);

            form.Code = message.Form.Code;
            form.CurrentVersion = message.Form.CurrentVersion;
            form.Description = message.Form.Description;
            form.FormSections = new List<FormSection>();

            foreach (var fs in message.Form.FormSections)
            {
                var formSection = MapFormSection(fs);
                form.FormSections.Add(formSection);
            }

            await _context.SaveChangesAsync();
            return message.Form;
        }

        private FormSection MapFormSection(FormSectionDTO formSectionDto)
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

        private Question MapQuestion(QuestionDTO questionDto)
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