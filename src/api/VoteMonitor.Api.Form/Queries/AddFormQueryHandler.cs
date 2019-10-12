using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class AddFormQueryHandler :
        AsyncRequestHandler<AddFormQuery, FormDTO>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public AddFormQueryHandler(VoteMonitorContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        protected override async Task<FormDTO> HandleCore(AddFormQuery message) {
            var newForm = new Entities.Form {
                Code = message.Form.Code,
                CurrentVersion = message.Form.CurrentVersion,
                Description = message.Form.Description
            };

            newForm.FormSections = new List<FormSection>();
            foreach (var fs in message.Form.FormSections) {
                var formSection = new FormSection { Code = fs.Code, Description = fs.Description };
                formSection.Questions = new List<Question>();
                foreach (var q in fs.Questions) {
                    var question = new Question{ QuestionType = q.QuestionType, Hint = q.Hint, Text = q.Text };
                    var optionsForQuestion = new List<OptionToQuestion>();
                    foreach (var o in q.OptionsToQuestions)
                        if (o.IdOption > 0) {
                            var existingOption = _context.Options.FirstOrDefault(option => option.Id == o.IdOption);
                            optionsForQuestion.Add(new OptionToQuestion { Option = existingOption });
                        }
                        else {
                            optionsForQuestion.Add(_mapper.Map<OptionToQuestion>(o));
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