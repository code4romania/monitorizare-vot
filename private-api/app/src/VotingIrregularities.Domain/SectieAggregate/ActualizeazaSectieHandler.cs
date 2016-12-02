using System;
using System.Threading.Tasks;
using MediatR;
using VotingIrregularities.Domain.Models;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace VotingIrregularities.Domain.SectieAggregate
{
    public class ActualizeazaSectieHandler : IAsyncRequestHandler<ActualizeazaSectieCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ActualizeazaSectieHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(ActualizeazaSectieCommand message)
        {
            try
            {
                var formular = await _context.RaspunsFormular
                    .FirstOrDefaultAsync(a =>
                        a.IdObservator == message.IdObservator &&
                        a.IdSectieDeVotare == message.IdSectieDeVotare);

                if (formular == null)
                    throw new ArgumentException("RaspunsFormular nu exista");
               
                _mapper.Map(message, formular);
                _context.Update(formular);

                return await _context.SaveChangesAsync();

            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(), ex.Message);
            }

            return -1;
        }
    }
}
