using MediatR;
using VoteMonitor.Api.PollingStation.Models;

namespace VoteMonitor.Api.PollingStation.Queries;

public record GetPollingStations(int CountyId, int Page, int PageSize) : IRequest<IEnumerable<GetPollingStationModel>>;
