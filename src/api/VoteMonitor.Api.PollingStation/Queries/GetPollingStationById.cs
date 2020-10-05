using MediatR;

namespace VoteMonitor.Api.PollingStation.Queries
{
    public class GetPollingStationById : IRequest<Models.GetPollingStation>
    {
        public int Id { get; set; }
    }
}
