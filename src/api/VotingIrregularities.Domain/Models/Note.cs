using System;

namespace VotingIrregularities.Domain.Models
{
    public partial class Note
    {
        public int Id { get; set; }
        public string AttachementPath { get; set; }
        public DateTime LastModified { get; set; }
        public int? IdQuestion { get; set; }
        public int IdObserver { get; set; }
        public int IdPollingStation { get; set; }
        public string Text { get; set; }

        public virtual Question Question { get; set; }
        public virtual Observer Observer { get; set; }
        public virtual PollingStation PollingStation { get; set; }
    }
}
