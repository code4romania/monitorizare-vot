using VoteMonitor.Api.Core;

namespace VoteMonitor.Api.Answer.Models
{
    public class SectionAnswersRequest : PagingModel
    {
        public bool Urgent { get; set; }
        public string County { get; set; }
        public int PollingStationNumber { get; set; }
        public int ObserverId { get; set; }
        public string ObserverPhoneNumber { get; set; }
    }

    public class ObserverAnswersRequest
    {
        public int PollingStationNumber { get; set; }
        public int ObserverId { get; set; }
    }
}
