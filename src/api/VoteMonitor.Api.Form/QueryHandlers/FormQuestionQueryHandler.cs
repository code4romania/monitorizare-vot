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
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers
{
    public class FormQuestionQueryHandler : IRequestHandler<FormQuestionQuery, IEnumerable<FormSectionDTO>>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormQuestionQueryHandler(VoteMonitorContext context, IMapper mapper, ICacheService cacheService)
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
                    var questions = await _context.Questions
                        .Include(a => a.FormSection)
                        .Include(a => a.OptionsToQuestions)
                        .ThenInclude(a => a.Option)
                        .Where(a => a.FormSection.Form.Id == message.FormId) // todo: FormCode might not be unique anymore - maybe we should query by FormId?
                        .ToListAsync();

                    var sections = questions.Select(q => new { q.IdSection, q.FormSection.Code, q.FormSection.Description, q.FormSection.OrderNumber })
                                    .Distinct()
                                    .OrderBy(s => s.OrderNumber);

                    var result = sections.Select(section => new FormSectionDTO
                    {
                        UniqueId = form.Code + section.Code + section.IdSection,
                        Id = section.IdSection,
                        OrderNumber = section.OrderNumber,
                        Code = section.Code,
                        Description = section.Description,
                        Questions = questions.Where(a => a.IdSection == section.IdSection)
                                     .OrderBy(question => question.OrderNumber)
                                     .Select(q=> OrderOptions(q))
                                     .Select(a => _mapper.Map<QuestionDTO>(a)).ToList()
                    }).ToList();

 
                    return result;
                },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                });
        }

        private static Question OrderOptions(Question q)
        {
            q.OptionsToQuestions = q.OptionsToQuestions.OrderBy(o => o.Option.OrderNumber).ToList();
            return q;
        }
    }
}
