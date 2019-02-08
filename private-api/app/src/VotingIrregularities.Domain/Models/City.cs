using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class City
    {
        public City()
        {
            PollingStations = new HashSet<PollingStation>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<PollingStation> PollingStations { get; set; }
    }
}
