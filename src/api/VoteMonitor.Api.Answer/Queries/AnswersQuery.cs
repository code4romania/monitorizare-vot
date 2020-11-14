using MediatR;
using VoteMonitor.Api.Answer.Models;
using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Answer.Queries
{
    public class AnswersQuery : PagingModel, IRequest<ApiListResponse<AnswerQueryDTO>>
    {
        public int IdONG { get; set; }
        public bool Urgent { get; set; }
        public bool Organizer { get; set; }
        public string County { get; set; }
        public int PollingStationNumber { get; set; }
        public int ObserverId { get; set; }
        public string ObserverPhoneNumber { get; set; }
    }
}