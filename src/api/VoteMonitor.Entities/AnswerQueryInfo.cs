using System;

namespace VoteMonitor.Entities
{
    public class AnswerQueryInfo
    {
        public int IdPollingStation { get; set; }
        public int IdObserver { get; set; }
        public string ObserverPhoneNumber { get; set; }
        public string ObserverName { get; set; }
        public string PollingStation { get; set; }
        public DateTime LastModified { get; set; }
    }
}
