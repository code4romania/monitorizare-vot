using System.Collections.Generic;
using MediatR;
using VoteMonitor.Api.Location.Models;

namespace VoteMonitor.Api.Location.Queries
{
    public class PollingStationsAssignmentQuery : IRequest<IEnumerable<CountyPollingStationLimit>>
    {
    }
}