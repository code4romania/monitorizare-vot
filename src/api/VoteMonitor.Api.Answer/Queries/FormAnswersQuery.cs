using MediatR;
using VoteMonitor.Api.Answer.Models;

namespace VoteMonitor.Api.Answer.Queries
{
    public class FormAnswersQuery : IRequest<PollingStationInfoDto>
    {
        public int PollingStationId { get; set; }
        public int ObserverId { get; set; }
    }
}