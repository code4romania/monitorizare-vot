using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using VoteMonitor.Api.Core.Services;
using VoteMonitor.Api.Form.Models;
using VoteMonitor.Api.Form.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Form.QueryHandlers;

public class FormQuestionQueryHandler : IRequestHandler<FormQuestionQuery, IEnumerable<FormSectionDTO>>
{
    private readonly VoteMonitorContext _context;
    private readonly ICacheService _cacheService;

    public FormQuestionQueryHandler(VoteMonitorContext context, ICacheService cacheService)
    {
        _context = context;
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
                    .Where(a => a.FormSection.Form.Id == message.FormId)
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
                        .Select(q => new QuestionDTO()
                        {
                            Code =q.Code,
                            Text = q.Text,
                            Hint = q.Hint,
                            FormCode = q.FormSection.Form.Code,
                            Id = q.Id,
                            QuestionType = q.QuestionType,
                            IdSection = q.IdSection,
                            OrderNumber = q.OrderNumber,
                            OptionsToQuestions = q.OptionsToQuestions.Select(x=> new OptionToQuestionDTO()
                            {
                                Text = x.Option.Text,
                                IsFreeText = x.Option.IsFreeText,
                                OrderNumber = x.Option.OrderNumber,
                                Flagged = x.Flagged,
                                OptionId = x.Id
                            }).ToList()
                        }).ToList()
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
