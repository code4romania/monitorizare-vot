using System;

namespace VotingIrregularities.Domain.Models
{
    public partial class PollingStationInfo
    {
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public DateTime LastModified { get; set; }
        public bool? UrbanArea { get; set; }
        public DateTime? ObserverLeaveTime { get; set; }
        public DateTime? ObserverArrivalTime { get; set; }
        public bool? IsPollingStationPresidentFemale { get; set; }

        public virtual Observer Observer { get; set; }
        public virtual PollingStation PollingStation { get; set; }
    }
}
