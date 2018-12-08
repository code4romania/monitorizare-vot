using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Answer
    {
        public int ObserverId { get; set; }
        public int AvailableAnswerId { get; set; }
        public int VotingSectionId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public string Value { get; set; }
        public string CountyCode { get; set; }
        public int SectionNumber { get; set; }

        public virtual Observer ObserverNavigationId { get; set; }
        public virtual AvailableAnswer AvailableAnswerNavigationId { get; set; }
        public virtual VotingSection VotingSectionNavigationId { get; set; }
    }
}
