using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Domain.Models;

namespace VotingIrregularities.Api.Queries
{
    public class FormularQueryHandler : IAsyncRequestHandler<ModelFormular.VersiuneQuery,Dictionary<string,int>>
    {
        private readonly VotingContext _context;

        public FormularQueryHandler(VotingContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> Handle(ModelFormular.VersiuneQuery message)
        {
            var result = await _context.VersiuneFormular
                .AsNoTracking()
                .ToListAsync();

            return result.ToDictionary(k => k.CodFormular, v => v.VersiuneaCurenta);
        }
    }
}
