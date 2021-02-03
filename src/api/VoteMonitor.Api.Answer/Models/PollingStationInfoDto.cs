using System;

namespace VoteMonitor.Api.Answer.Models
{
    public class PollingStationInfoDto
    {
        public DateTime LastModified { get; set; }
        public bool? UrbanArea { get; set; }
        public DateTime? ObserverLeaveTime { get; set; }
        public DateTime? ObserverArrivalTime { get; set; }
        public bool? IsPollingStationPresidentFemale { get; set; }
    }
}
