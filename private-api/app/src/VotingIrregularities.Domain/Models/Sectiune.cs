using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Sectiune
    {
        public Sectiune()
        {
            Intrebare = new HashSet<Intrebare>();
        }

        public string CodSectiune { get; set; }
        public string Descriere { get; set; }

        public virtual ICollection<Intrebare> Intrebare { get; set; }
    }
}
