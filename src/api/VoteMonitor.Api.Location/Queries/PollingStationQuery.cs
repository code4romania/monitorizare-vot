using MediatR;

namespace VoteMonitor.Api.Location.Queries {
    public class PollingStationQuery : IRequest<int> {
        public string CountyCode { get; set; }
        public int IdPollingStation { get; set; }
    }
}
