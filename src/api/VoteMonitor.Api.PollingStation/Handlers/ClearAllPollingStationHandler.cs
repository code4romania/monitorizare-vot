using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Services;
using VoteMonitor.Api.PollingStation.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers
{
    public class ClearAllPollingStationHandler : IRequestHandler<ClearAllPollingStationsCommand, int>
    {
        private readonly VoteMonitorContext _context;
        private readonly ILogger<ClearAllPollingStationHandler> _logger;

        public ClearAllPollingStationHandler(VoteMonitorContext context, ILogger<ClearAllPollingStationHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(ClearAllPollingStationsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                int result = 0;
                using (var transaction = await _context.Database.BeginTransactionAsync(cancellationToken))
                {
                    //if there are any performance issues, DELETE can be changed to TRUNCATE - but then we need to tackle a way to 
                    //bypass foreign key relationship while removing data
                    await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(_context.Answers)}"
                        , cancellationToken);
                    await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(_context.Notes)}"
                        , cancellationToken);
                    await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(_context.PollingStationInfos)}"
                        , cancellationToken);
                    await _context.Database.ExecuteSqlRawAsync($"UPDATE {nameof(_context.Counties)} SET {nameof(County.NumberOfPollingStations)} = 0"
                        , cancellationToken);
                    result = await _context.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(_context.PollingStations)}"
                        , cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while removing polling stations.", ex);
            }

            return -1;
        }
    }
}
