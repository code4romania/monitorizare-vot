namespace VoteMonitor.Api.Answer.Models
{
    public class AnswerQueryDTO
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public string ObserverName { get; set; }
        public string ObserverPhoneNumber { get; set; }
        public string PollingStationName { get; set; }
    }
}
