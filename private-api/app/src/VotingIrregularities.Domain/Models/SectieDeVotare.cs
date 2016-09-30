using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class SectieDeVotare
    {
        public SectieDeVotare()
        {
            Nota = new HashSet<Nota>();
            Raspuns = new HashSet<Raspuns>();
            RaspunsFormular = new HashSet<RaspunsFormular>();
        }

        public int IdSectieDeVotarre { get; set; }
        public string AdresaSectie { get; set; }
        public string Coordonate { get; set; }
        public string DenumireUat { get; set; }
        public int IdJudet { get; set; }
        public string LocalitateComponenta { get; set; }
        public int NumarSectie { get; set; }

        public virtual ICollection<Nota> Nota { get; set; }
        public virtual ICollection<Raspuns> Raspuns { get; set; }
        public virtual ICollection<RaspunsFormular> RaspunsFormular { get; set; }
        public virtual Judet IdJudetNavigation { get; set; }
    }
}
