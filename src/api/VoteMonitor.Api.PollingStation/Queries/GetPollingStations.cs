using System.Collections.Generic;
using MediatR;
using VoteMonitor.Api.PollingStation.Models;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class GetPollingStations : IRequest<IEnumerable<GetPollingStationModel>>
    {
        public int CountyId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}