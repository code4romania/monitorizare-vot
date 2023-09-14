using MediatR;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Location.Queries;

public record PollingStationsAssignmentQuery(bool? Diaspora) : IRequest<IEnumerable<CountyPollingStationLimit>>;
