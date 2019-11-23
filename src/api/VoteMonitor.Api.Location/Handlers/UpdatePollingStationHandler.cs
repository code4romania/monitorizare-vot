using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers
{
    public class UpdatePollingSectionHandler : IRequestHandler<UpdatePollingSectionCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public UpdatePollingSectionHandler(VoteMonitorContext context, ILogger logger, IMapper mapper)
        {
            _context = context;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<int> Handle(UpdatePollingSectionCommand message, CancellationToken cancellationToken)
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
