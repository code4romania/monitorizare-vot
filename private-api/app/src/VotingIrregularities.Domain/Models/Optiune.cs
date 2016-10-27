using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Optiune
    {
        public Optiune()
        {
            RaspunsDisponibil = new HashSet<RaspunsDisponibil>();
        }

        public int IdOptiune { get; set; }
        public bool SeIntroduceText { get; set; }
        public string TextOptiune { get; set; }

        public virtual ICollection<RaspunsDisponibil> RaspunsDisponibil { get; set; }
    }
}
