using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class CheckPollingStationExists: IRequest<bool>
    {
        public int PollingStationId { get; set; }
    }
}
