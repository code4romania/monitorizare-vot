using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Ong
    {
        public Ong()
        {
            AdminOng = new HashSet<AdminOng>();
            Observator = new HashSet<Observator>();
        }

        public int IdOng { get; set; }
        public string AbreviereNumeOng { get; set; }
        public string NumeOng { get; set; }
        public bool Organizator { get; set; }

        public virtual ICollection<AdminOng> AdminOng { get; set; }
        public virtual ICollection<Observator> Observator { get; set; }
    }
}
