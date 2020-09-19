using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.Queries
{
    public class FormQueryHandler :
        IRequestHandler<FormQuestionQuery, IEnumerable<FormSectionDTO>>,
        IRequestHandler<DeleteFormCommand, bool>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormQueryHandler(VoteMonitorContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<FormSectionDTO>> Handle(FormQuestionQuery message, CancellationToken cancellationToken)
        {
            var form = _context.Forms.FirstOrDefault(f => f.Id == message.FormId);
            if (form == null)
            {
                return null;
            }

            var cacheKey = $"Formular{form.Code}";

            return await _cacheService.GetOrSaveDataInCacheAsync<IEnumerable<FormSectionDTO>>(cacheKey,
                async () =>
                {
                    var r = await _context.Questions
                        .Include(a => a.FormSection)
                        .Include(a => a.OptionsToQuestions)
                        .ThenInclude(a => a.Option)
                        .Where(a => a.FormSection.Form.Id == message.FormId) // todo: FormCode might not be unique anymore - maybe we should query by FormId?
                        .ToListAsync();

                    var sectiuni = r.Select(a => new { IdSectiune = a.IdSection, CodSectiune = a.FormSection.Code, Descriere = a.FormSection.Description }).Distinct();

                    var result = sectiuni.Select(i => new FormSectionDTO
                    {
                        UniqueId = form.Code + i.CodSectiune + i.IdSectiune,
                        Code = i.CodSectiune,
                        Description = i.Descriere,
                        Questions = r.Where(a => a.IdSection == i.IdSectiune)
                                     .OrderBy(intrebare => intrebare.Code)
                                     .Select(a => _mapper.Map<QuestionDTO>(a)).ToList()
                    }).ToList();
                    return result;
                },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                });
        }

        public async Task<bool> Handle(DeleteFormCommand request, CancellationToken cancellationToken)
        {
            var form = await _context.Forms.FirstOrDefaultAsync(f => f.Id == request.FormId);
            if (form == null)
            {
                return false;
            }

            var sections = _context.FormSections.Where(s => s.IdForm == form.Id);
            var sectionsIds = sections.Select(s => s.Id);
            var questions = _context.Questions.Where(q => sectionsIds.Contains(q.IdSection));
            var questionsIds = questions.Select(q => q.Id);
            var optionsToQuestions = _context.OptionsToQuestions.Where(o => questionsIds.Contains(o.IdQuestion));
            var optionsIds = optionsToQuestions.Select(o => o.IdOption);
            var options = _context.Options.Where(o => optionsIds.Contains(o.Id));

            _context.OptionsToQuestions.RemoveRange(optionsToQuestions);
            _context.Options.RemoveRange(options);
            _context.Questions.RemoveRange(questions);
            _context.FormSections.RemoveRange(sections);
            _context.Forms.Remove(form);

            await _context.SaveChangesAsync();
            return true;
        }
    }
}