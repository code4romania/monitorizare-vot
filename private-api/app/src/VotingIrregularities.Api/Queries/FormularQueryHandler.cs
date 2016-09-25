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
    public class FormularQueryHandler : IAsyncRequestHandler<ModelFormular.VersiuneQuery,int?>
    {
        private readonly VotingContext _context;

        public FormularQueryHandler(VotingContext context)
        {
            _context = context;
        }

        public async Task<int?> Handle(ModelFormular.VersiuneQuery message)
        {
            var result = await _context.VersiuneFormular
                .FirstOrDefaultAsync(f => f.CodFormular == message.CodFormular);

            return result?.VersiuneaCurenta;
        }
    }
}
