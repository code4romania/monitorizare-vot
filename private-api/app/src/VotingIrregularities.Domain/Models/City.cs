using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class City
    {
        public City()
        {
            VotingSections = new HashSet<VotingSection>();
        }

        public int CityId { get; set; }
        public string CityName { get; set; }

        public virtual ICollection<VotingSection> VotingSections { get; set; }
    }
}
