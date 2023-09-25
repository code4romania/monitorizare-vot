using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries;

public record CheckPollingStationExists(int PollingStationId) : IRequest<bool>;
