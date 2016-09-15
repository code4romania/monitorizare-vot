using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Optiune
    {
        public Optiune()
        {
            Raspuns = new HashSet<Raspuns>();
            RaspunsDisponibil = new HashSet<RaspunsDisponibil>();
        }

        public int IdOptiune { get; set; }
        public string TextOptiune { get; set; }
        public bool SeIntroduceText { get; set; }

        public virtual ICollection<Raspuns> Raspuns { get; set; }
        public virtual ICollection<RaspunsDisponibil> RaspunsDisponibil { get; set; }
    }
}
