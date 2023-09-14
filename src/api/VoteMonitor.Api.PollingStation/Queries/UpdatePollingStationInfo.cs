using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries;

public record UpdatePollingStationInfo(int ObserverId, int PollingStationId, DateTime ObserverLeaveTime) : IRequest
{
    public int ObserverId { get; set; } = ObserverId;
    public int PollingStationId { get; set; } = PollingStationId;
    public DateTime ObserverLeaveTime { get; set; } = ObserverLeaveTime;
}
