using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries;

public record CreatePollingStationInfo(int ObserverId, int PollingStationId, string CountyCode, DateTime? ObserverLeaveTime, DateTime? ObserverArrivalTime, bool? IsPollingStationPresidentFemale) : IRequest;
