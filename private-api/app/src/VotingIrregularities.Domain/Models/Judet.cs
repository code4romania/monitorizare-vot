using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Judet
    {
        public Judet()
        {
            SectieDeVotare = new HashSet<SectieDeVotare>();
        }

        public int IdJudet { get; set; }
        public string CodJudet { get; set; }
        public string Nume { get; set; }

        public virtual ICollection<SectieDeVotare> SectieDeVotare { get; set; }
    }
}
