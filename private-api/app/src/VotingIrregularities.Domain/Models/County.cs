using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class County
    {
        public County()
        {
            PollingStations = new HashSet<PollingStation>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int NumberOfPollingStations { get; set; }

        public virtual ICollection<PollingStation> PollingStations { get; set; }
    }
}
