using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class AccessOfObserversPerDevice
    {
        public int ObserverId { get; set; }
        public string MobileDeviceId { get; set; }
        public DateTime RegisterDateOfTheDevice { get; set; }

        public virtual Observer ObserverNavigationId { get; set; }
    }
}
