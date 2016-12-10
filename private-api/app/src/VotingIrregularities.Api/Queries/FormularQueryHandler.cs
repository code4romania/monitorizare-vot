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
        IAsyncRequestHandler<ModelFormular.VersiuneQuery,Dictionary<string,int>>,
        IAsyncRequestHandler<ModelFormular.IntrebariQuery,IEnumerable<ModelSectiune>>
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

        public async Task<Dictionary<string, int>> Handle(ModelFormular.VersiuneQuery message)
        {
            var result = await _context.VersiuneFormular
                .AsNoTracking()
                .ToListAsync();

            return result.ToDictionary(k => k.CodFormular, v => v.VersiuneaCurenta);
        }

        public async Task<IEnumerable<ModelSectiune>> Handle(ModelFormular.IntrebariQuery message)
        {
            CacheObjectsName formular;
            Enum.TryParse("Formular" + message.CodFormular, out formular);

            return await _cacheService.GetOrSaveDataInCacheAsync<IEnumerable<ModelSectiune>>(formular,
                async () =>
                {
                    var r = await _context.Intrebare
                        .Include(a => a.IdSectiuneNavigation)
                        .Include(a => a.RaspunsDisponibil)
                        .ThenInclude(a => a.IdOptiuneNavigation)
                        .Where(a => a.CodFormular == message.CodFormular)
                        .ToListAsync();

                    var sectiuni = r.Select(a => new { a.IdSectiune, a.IdSectiuneNavigation.CodSectiune, a.IdSectiuneNavigation.Descriere }).Distinct();

                    var result = sectiuni.Select(i => new ModelSectiune
                    {
                        CodSectiune = i.CodSectiune,
                        Descriere = i.Descriere,
                        Intrebari = r.Where(a => a.IdSectiune == i.IdSectiune)
                                     .OrderBy(intrebare => intrebare.CodIntrebare)
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