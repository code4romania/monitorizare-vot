using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Oras
    {
        public Oras()
        {
            SectieDeVotare = new HashSet<SectieDeVotare>();
        }

        public int IdOras { get; set; }
        public string NumeOras { get; set; }

        public virtual ICollection<SectieDeVotare> SectieDeVotare { get; set; }
    }
}
