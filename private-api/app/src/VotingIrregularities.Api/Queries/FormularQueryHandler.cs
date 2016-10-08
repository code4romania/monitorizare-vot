using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class FormularQueryHandler :
        IAsyncRequestHandler<ModelFormular.VersiuneQuery,Dictionary<string,int>>,
        IAsyncRequestHandler<ModelFormular.IntrebariQuery,IEnumerable<ModelSectiune>>
    {
        private readonly VotingContext _context;
        private readonly IConfigurationProvider _provider;

        public FormularQueryHandler(VotingContext context, IConfigurationProvider provider)
        {
            _context = context;
            _provider = provider;
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
            var result = await _context.Sectiune
                .Include(a => a.Intrebare)
                    .ThenInclude(a => a.RaspunsDisponibil)
                    .ThenInclude(a => a.IdOptiuneNavigation)
                .Where(a => a.Intrebare.All( i => i.CodFormular == message.CodFormular))
                .ProjectTo<ModelSectiune>(_provider)
                .OrderBy(a => a.CodSectiune)
                .ToListAsync();

            return result;

        }
    }
}
