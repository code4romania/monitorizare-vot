using System.Collections.Generic;

namespace VoteMonitor.Entities
{
    public class PollingStation : IIdentifiableEntity
    {
        public PollingStation()
        {
            Notes = new HashSet<Note>();
            Answers = new HashSet<Answer>();
            PollingStationInfos = new HashSet<PollingStationInfo>();
        }

        public int Id { get; set; }
        public int IdCounty { get; set; }
        public int Number { get; set; }
        public string Address { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<PollingStationInfo> PollingStationInfos { get; set; }
        public virtual County County { get; set; }
    }
}
