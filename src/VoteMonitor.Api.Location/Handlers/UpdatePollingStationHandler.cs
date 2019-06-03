using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Location.Commands;
using VotingIrregularities.Domain.Models;
using VotingIrregularities.Domain.SectieAggregate;

namespace VoteMonitor.Api.Location.Handlers
{
    public class UpdatePollingSectionHandler : AsyncRequestHandler<UpdatePollingSectionCommand, int>
    {
        private readonly VotingContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UpdatePollingSectionHandler(VotingContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        protected override async Task<int> HandleCore(UpdatePollingSectionCommand message)
        {
            try
            {
                var pollingStationInfo = await _context.PollingStationInfos
                    .FirstOrDefaultAsync(a =>
                        a.IdObserver == message.IdObserver &&
                        a.IdPollingStation == message.IdPollingStation);

                if (pollingStationInfo == null)
                    throw new ArgumentException("PollingStationInfo nu exista");
               
                _mapper.Map(message, pollingStationInfo);
                _context.Update(pollingStationInfo);

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
