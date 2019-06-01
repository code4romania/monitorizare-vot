using MediatR;

namespace VotingIrregularities.Api.Models {
    public class PollingStationQuery : IRequest<int> {
        public string CountyCode { get; set; }
        public int IdPollingStation { get; set; }
    }
}
