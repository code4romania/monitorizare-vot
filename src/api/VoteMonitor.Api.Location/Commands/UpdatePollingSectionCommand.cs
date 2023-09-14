using MediatR;

namespace VoteMonitor.Api.Location.Commands;

public class UpdatePollingSectionCommand : IRequest<int>
{
    public int ObserverId { get; set; }
    public int PollingStationId { get; set; }
    public DateTime ObserverLeaveTime { get; set; }
}