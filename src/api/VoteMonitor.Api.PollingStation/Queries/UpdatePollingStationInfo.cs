using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries;

public record UpdatePollingStationInfo(int ObserverId, int PollingStationId, DateTime ObserverLeaveTime) : IRequest;
