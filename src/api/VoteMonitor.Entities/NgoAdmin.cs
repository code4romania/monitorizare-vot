using System.Collections.Generic;

namespace VoteMonitor.Entities
{
    public partial class NgoAdmin : IIdentifiableEntity
    {
        public int Id { get; set; }
        public int IdNgo { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }

        public virtual Ngo Ngo { get; set; }
        public virtual ICollection<Notification> NotificationsSent { get; set; }

    }
}
