using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.RaspunsAggregate.Commands;

namespace VotingIrregularities.Domain.RaspunsAggregate
{
    public class CompleteazaRaspunsHandler : IAsyncRequestHandler<CompleteazaRaspunsCommand, int>
    {
        private readonly VotingContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CompleteazaRaspunsHandler(VotingContext context, IMapper mapper, ILogger logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<int> Handle(CompleteazaRaspunsCommand message)
        {
            return await Task.FromResult(0);
        }
    }
}
