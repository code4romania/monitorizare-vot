using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class CheckPollingStationExistsHandler: IRequestHandler<CheckPollingStationExists, bool>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public CheckPollingStationExistsHandler(VoteMonitorContext context, ILogger<CheckPollingStationExistsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> Handle(CheckPollingStationExists request, CancellationToken cancellationToken)
        {
            try
            {
                return await _context.PollingStations.AnyAsync(p => p.Id == request.PollingStationId, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving polling station: ", ex);
                throw;
            }
        }
    }
}
