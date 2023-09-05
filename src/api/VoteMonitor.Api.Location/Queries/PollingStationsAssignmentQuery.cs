using MediatR;
using VoteMonitor.Api.Core.Models;

namespace VoteMonitor.Api.Location.Queries;

public class PollingStationsAssignmentQuery : IRequest<IEnumerable<CountyPollingStationLimit>>
{
    public bool? Diaspora { get; }

    public PollingStationsAssignmentQuery(bool? diaspora)
    {
        Diaspora = diaspora;
    }
}
