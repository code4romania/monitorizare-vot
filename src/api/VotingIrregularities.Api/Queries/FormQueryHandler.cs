using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Models.Forms;
using VotingIrregularities.Api.Services;
using VoteMonitor.Entities;

namespace VotingIrregularities.Api.Queries
{
    public class FormQueryHandler :
        AsyncRequestHandler<FormQuestionQuery, IEnumerable<FormSectionDTO>>
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

        protected override async Task<IEnumerable<FormSectionDTO>> HandleCore(FormQuestionQuery message)
        {
            CacheObjectsName formular;
            Enum.TryParse("Formular" + message.FormCode, out formular);

            return await _cacheService.GetOrSaveDataInCacheAsync<IEnumerable<FormSectionDTO>>(formular,
                async () =>
                {
                    var r = await _context.Questions
                        .Include(a => a.FormSection)
                        .Include(a => a.OptionsToQuestions)
                        .ThenInclude(a => a.Option)
                        .Where(a => a.FormCode == message.FormCode)
                        .ToListAsync();

                    var sectiuni = r.Select(a => new { IdSectiune = a.IdSection, CodSectiune = a.FormSection.Code, Descriere = a.FormSection.Description }).Distinct();

                    var result = sectiuni.Select(i => new FormSectionDTO {
                        UniqueId = message.FormCode + i.CodSectiune + i.IdSectiune ,
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
    }
}