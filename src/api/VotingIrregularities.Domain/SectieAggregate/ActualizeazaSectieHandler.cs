using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VoteMonitor.Entities;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace VotingIrregularities.Domain.SectieAggregate
{
    public class ActualizeazaSectieHandler : IRequestHandler<ActualizeazaSectieCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ActualizeazaSectieHandler(VoteMonitorContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(ActualizeazaSectieCommand message, CancellationToken cancellationToken)
        {
            try
            {
                var formular = await _context.PollingStationInfos
                    .FirstOrDefaultAsync(a =>
                        a.IdObserver == message.IdObservator &&
                        a.IdPollingStation == message.IdSectieDeVotare);

                if (formular == null)
                    throw new ArgumentException("PollingStationInfo nu exista");

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
