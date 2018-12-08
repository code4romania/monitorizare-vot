using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class County
    {
        public County()
        {
            VotingSections = new HashSet<VotingSection>();
        }

        public int CountyId { get; set; }
        public string CountyCode { get; set; }
        public string Name { get; set; }

        public virtual ICollection<VotingSection> VotingSections { get; set; }
    }
}
