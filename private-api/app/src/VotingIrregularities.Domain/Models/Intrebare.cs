using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Intrebare
    {
        public Intrebare()
        {
            Nota = new HashSet<Nota>();
            Raspuns = new HashSet<Raspuns>();
            RaspunsDisponibil = new HashSet<RaspunsDisponibil>();
        }

        public int IdIntrebare { get; set; }
        public string CodFormular { get; set; }
        public int IdSectiune { get; set; }
        public int IdTipIntrebare { get; set; }
        public string TextIntrebare { get; set; }

        public virtual ICollection<Nota> Nota { get; set; }
        public virtual ICollection<Raspuns> Raspuns { get; set; }
        public virtual ICollection<RaspunsDisponibil> RaspunsDisponibil { get; set; }
        public virtual Sectiune IdSectiuneNavigation { get; set; }
    }
}
