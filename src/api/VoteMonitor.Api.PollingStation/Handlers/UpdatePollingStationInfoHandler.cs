using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers;

public class UpdatePollingStationInfoHandler : IRequestHandler<UpdatePollingStationInfo>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public UpdatePollingStationInfoHandler(VoteMonitorContext context, ILogger<UpdatePollingStationInfoHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UpdatePollingStationInfo request, CancellationToken cancellationToken)
    {
        try
        {
            var pollingStationInfo = _context.PollingStationInfos.FirstOrDefault(
                p => p.IdObserver == request.ObserverId &&
                     p.IdPollingStation == request.PollingStationId);

            if(pollingStationInfo == null)
            {
                return;
            }

            pollingStationInfo.ObserverLeaveTime = request.ObserverLeaveTime;
            pollingStationInfo.LastModified = DateTime.Now;

            _context.Update(pollingStationInfo);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating polling station info {@request}", request);
            throw;
        }
    }
}
