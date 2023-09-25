using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries;

public record UpdatePollingStation(int PollingStationId, string Address, int Number) : IRequest<bool?>;
