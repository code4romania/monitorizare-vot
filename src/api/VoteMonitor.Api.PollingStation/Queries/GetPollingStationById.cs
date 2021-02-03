using MediatR;
using VoteMonitor.Api.PollingStation.Models;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class GetPollingStationById : IRequest<GetPollingStationModel>
    {
        public int PollingStationId { get; set; }
    }
}
