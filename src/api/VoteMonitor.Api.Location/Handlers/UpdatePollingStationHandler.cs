using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.Location.Commands;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.Location.Handlers;

public class UpdatePollingSectionHandler : IRequestHandler<UpdatePollingSectionCommand, int>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public UpdatePollingSectionHandler(VoteMonitorContext context, ILogger<UpdatePollingSectionHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(UpdatePollingSectionCommand message, CancellationToken cancellationToken)
    {
        try
        {
            var pollingStationInfo = await _context.PollingStationInfos
                .FirstOrDefaultAsync(a =>
                    a.IdObserver == message.ObserverId &&
                    a.IdPollingStation == message.PollingStationId);

            if (pollingStationInfo == null)
            {
                throw new ArgumentException($"PollingStationInfo for observerId =  {message.ObserverId} idPollingStation = {message.PollingStationId}");
            }

            pollingStationInfo.ObserverLeaveTime = message.ObserverLeaveTime;
            pollingStationInfo.LastModified = DateTime.UtcNow;
            
            _context.Update(pollingStationInfo);

            return await _context.SaveChangesAsync();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }

        return -1;
    }
}
