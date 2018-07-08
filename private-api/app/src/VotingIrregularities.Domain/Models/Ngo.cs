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

        public int Id { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public bool Organizer { get; set; }

        public virtual ICollection<NgoAdmin> NgoAdmins { get; set; }
        public virtual ICollection<Observer> Observers { get; set; }
    }
}
