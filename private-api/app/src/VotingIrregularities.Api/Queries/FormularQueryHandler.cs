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
    public class FormularQueryHandler :
        AsyncRequestHandler<FormQuestionsQuery, IEnumerable<ModelSectiune>>
    {
        private readonly VotingContext _context;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public FormularQueryHandler(VotingContext context, IMapper mapper, ICacheService cacheService)
        {
            _context = context;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        protected override async Task<IEnumerable<ModelSectiune>> HandleCore(FormQuestionsQuery message)
        {
            CacheObjectsName formular;
            Enum.TryParse("Formular" + message.CodFormular, out formular);

            return await _cacheService.GetOrSaveDataInCacheAsync<IEnumerable<ModelSectiune>>(formular,
                async () =>
                {
                    var r = await _context.Questions
                        .Include(a => a.FormSection)
                        .Include(a => a.OptionsToQuestions)
                        .ThenInclude(a => a.Option)
                        .Where(a => a.FormCode == message.CodFormular)
                        .ToListAsync();

                    var sectiuni = r.Select(a => new { IdSectiune = a.IdSection, CodSectiune = a.FormSection.Code, Descriere = a.FormSection.Description }).Distinct();

                    var result = sectiuni.Select(i => new ModelSectiune
                    {
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