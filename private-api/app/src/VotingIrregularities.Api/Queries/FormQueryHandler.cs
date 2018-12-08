using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class FormQueryHandler :
        IAsyncRequestHandler<FormModel.VersionQuery, Dictionary<string, int>>,
        IAsyncRequestHandler<FormModel.QuestionsQuery,IEnumerable<SectionModel>>
    {
        private readonly VotingContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormQueryHandler(VotingContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<Dictionary<string, int>> Handle(FormModel.VersionQuery message)
        {
            var result = await _context.FormVersions
                .AsNoTracking()
                .ToListAsync();

            return result.ToDictionary(k => k.FormCode, v => v.CurrentVersion);
        }

        public async Task<IEnumerable<SectionModel>> Handle(FormModel.QuestionsQuery message)
        {
            CacheObjectsName form;
            Enum.TryParse("Formular" + message.FormCode, out form);

            return await _cacheService.GetOrSaveDataInCacheAsync<IEnumerable<SectionModel>>(form,
                async () =>
                {
                    var r = await _context.Questions
                        .Include(a => a.NavigationSectionId)
                        .Include(a => a.AvailableAnswer)
                        .ThenInclude(a => a.OptionNavigationId)
                        .Where(a => a.FormCode == message.FormCode)
                        .ToListAsync();

                    var sections = r.Select(a => new { IdSectiune = a.SectionId, SectionCode = a.NavigationSectionId.SectionCode, Description = a.NavigationSectionId.Description }).Distinct();

                    var result = sections.Select(i => new SectionModel
                    {
                        SectionCode = i.SectionCode,
                        Description = i.Description,
                        Questions = r.Where(a => a.SectionId == i.IdSectiune)
                                     .OrderBy(question => question.QuestionCode)
                                     .Select(a => _mapper.Map<QuestionModel>(a)).ToList()
                    }).ToList();

                    return result;
                },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = new TimeSpan(message.CacheHours, message.CacheMinutes, message.CacheMinutes)
                });
        }
    }
}