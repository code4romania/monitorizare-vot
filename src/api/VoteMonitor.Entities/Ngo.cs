using System.Collections.Generic;

namespace VoteMonitor.Entities
{
    public partial class Ngo : IIdentifiableEntity
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
        public bool IsActive { get; set; }

        public virtual ICollection<NgoAdmin> NgoAdmins { get; set; }
        public virtual ICollection<Observer> Observers { get; set; }
    }
}
