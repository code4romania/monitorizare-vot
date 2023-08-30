using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class UpdatePollingStation : IRequest<bool?>
    {
        public int PollingStationId { get; set; }
        public string Address { get; set; }
        public int Number { get; set; }
    }
}
