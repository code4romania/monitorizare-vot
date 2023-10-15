using MediatR;
using Microsoft.Extensions.Logging;
using VoteMonitor.Api.PollingStation.Queries;
using VoteMonitor.Entities;

namespace VoteMonitor.Api.PollingStation.Handlers;

public class CreatePollingStationInfoHandler : IRequestHandler<CreatePollingStationInfo>
{
    private readonly VoteMonitorContext _context;
    private readonly ILogger _logger;

    public CreatePollingStationInfoHandler(VoteMonitorContext context, ILogger<CreatePollingStationInfoHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(CreatePollingStationInfo request, CancellationToken cancellationToken)
    {
        try
        {
            var pollingStationInfo = new PollingStationInfo()
            {
                IdObserver = request.ObserverId,
                IdPollingStation = request.PollingStationId,
                ObserverArrivalTime = request.ObserverArrivalTime,
                ObserverLeaveTime = request.ObserverLeaveTime,
                LastModified = DateTime.UtcNow
            };

            _context.PollingStationInfos.Add(pollingStationInfo);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating polling station info: {@request}", request);
            throw;
        }
    }
}
