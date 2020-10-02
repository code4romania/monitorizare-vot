using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class CheckPollingStationExists: IRequest<bool>
    {
        public int Id { get; set; }
    }
}
