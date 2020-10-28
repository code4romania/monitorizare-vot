using System;
using System.Collections.Generic;

namespace VoteMonitor.Entities
{
    public partial class Note : IIdentifiableEntity
    {
        public int Id { get; set; }
        public DateTime LastModified { get; set; }
        public int? IdQuestion { get; set; }
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public string Text { get; set; }

        public virtual Question Question { get; set; }
        public virtual Observer Observer { get; set; }
        public virtual PollingStation PollingStation { get; set; }
        public virtual ICollection<NotesAttachments> Attachments { get; set; }

    }
}
