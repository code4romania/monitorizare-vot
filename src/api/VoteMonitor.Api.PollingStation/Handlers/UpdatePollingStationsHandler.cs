using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class UpdatePollingStationsHandler : IRequestHandler<UpdatePollingStation, bool?>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger _logger;

        public UpdatePollingStationsHandler(VoteMonitorContext context, ILogger<UpdatePollingStationsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool?> Handle(UpdatePollingStation request, CancellationToken cancellationToken)
        {
            try
            {
                var existingPollingStation = _context.PollingStations.FirstOrDefault(p => p.Id == request.PollingStationId);
                if (existingPollingStation == null)
                {
                    return false;
                }

                existingPollingStation.AdministrativeTerritoryCode = request.AdministrativeTerritoryCode;
                existingPollingStation.Coordinates = request.Coordinates;
                existingPollingStation.Address = request.Address;
                existingPollingStation.TerritoryCode = request.TerritoryCode;
                existingPollingStation.Number = request.Number;
                await _context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.Log(LogLevel.Error, $"Error updating polling station: {request.PollingStationId}",  ex);
                return null;
            }
            return true;
        }
    }
}
