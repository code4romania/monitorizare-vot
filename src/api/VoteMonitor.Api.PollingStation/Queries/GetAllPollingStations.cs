using System.Collections.Generic;
using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class GetAllPollingStations : IRequest<IEnumerable<Models.PollingStation>>
    {
    }
}