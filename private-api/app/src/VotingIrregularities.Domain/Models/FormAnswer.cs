using System;

namespace VotingIrregularities.Domain.Models
{
    public partial class FormAnswer
    {
        public int ObserverId { get; set; }
        public int VotingSectionId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public bool? IsUrbanArea { get; set; }
        public DateTime? LeaveDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public bool? BesvPresidentIsWoman { get; set; }

        public virtual Observer ObserverNavigationId { get; set; }
        public virtual VotingSection VotingSectionNavigationId { get; set; }
    }
}
