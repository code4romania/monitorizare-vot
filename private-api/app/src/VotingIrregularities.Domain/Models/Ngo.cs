using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Ngo
    {
        public Ngo()
        {
            NgoAdmins = new HashSet<NgoAdmin>();
            Observers = new HashSet<Observer>();
        }

        public int IdOng { get; set; }
        public string NgoNameAbbreviation { get; set; }
        public string NgoName { get; set; }
        public bool Organizer { get; set; }

        public virtual ICollection<NgoAdmin> NgoAdmins { get; set; }
        public virtual ICollection<Observer> Observers { get; set; }
    }
}
