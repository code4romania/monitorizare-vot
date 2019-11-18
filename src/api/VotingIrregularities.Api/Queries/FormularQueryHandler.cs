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

namespace VotingIrregularities.Api.Queries
{
    [Obsolete]
    public class FormularQueryHandler :
        IRequestHandler<FormQuestionsQuery, IEnumerable<ModelSectiune>>
    {
        private readonly VoteMonitorContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormularQueryHandler(VoteMonitorContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<IEnumerable<ModelSectiune>> Handle(FormQuestionsQuery message, CancellationToken cancellationToken)
        {
            var cacheKey = $"Formular{message.CodFormular}";

            return await _cacheService.GetOrSaveDataInCacheAsync<IEnumerable<ModelSectiune>>(cacheKey,
                async () =>
                {
                    var r = await _context.Questions
                        .Include(a => a.FormSection)
                        .Include(a => a.OptionsToQuestions)
                        .ThenInclude(a => a.Option)
                        .Where(a => a.FormSection.Form.Code == message.CodFormular) // todo: maybe we should query by FormId, since Form Code might not be unique if we have verions of the same form
                        .ToListAsync(cancellationToken: cancellationToken);

                    var sectiuni = r.Select(a => new { IdSectiune = a.IdSection, CodSectiune = a.FormSection.Code, Descriere = a.FormSection.Description }).Distinct();

                    var result = sectiuni.Select(i => new ModelSectiune
                    {
                        IdSectiune = message.CodFormular + i.CodSectiune + i.IdSectiune ,
                        CodSectiune = i.CodSectiune,
                        Descriere = i.Descriere,
                        Intrebari = r.Where(a => a.IdSection == i.IdSectiune)
                                     .OrderBy(intrebare => intrebare.Code)
                                     .Select(a => _mapper.Map<ModelIntrebare>(a)).ToList()
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