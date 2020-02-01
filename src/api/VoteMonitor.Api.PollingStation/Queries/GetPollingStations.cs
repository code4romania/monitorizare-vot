using System.Collections.Generic;
using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class GetPollingStations : IRequest<IEnumerable<Models.PollingStation>>
    {
        public int IdCounty { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}