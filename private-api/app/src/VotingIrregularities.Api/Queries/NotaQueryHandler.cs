using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using VotingIrregularities.Api.Models;
using VotingIrregularities.Api.Services;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.NotaAggregate;

namespace VotingIrregularities.Api.Queries
{
    public class NotaQueryHandler : IAsyncRequestHandler<ModelNoteBulk, AdaugaNotaCommand>
    {
        private readonly VotingContext _context;
        private readonly ISectieDeVotareService _service;

        public NotaQueryHandler(VotingContext context, ISectieDeVotareService service)
        {
            _context = context;
            _service = service;
        }
        public async Task<AdaugaNotaCommand> Handle(ModelNoteBulk message)
        {
            return await Task.FromResult(new AdaugaNotaCommand());
        }
    }
}
