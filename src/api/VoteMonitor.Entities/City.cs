using System.Collections.Generic;

namespace VoteMonitor.Entities
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
