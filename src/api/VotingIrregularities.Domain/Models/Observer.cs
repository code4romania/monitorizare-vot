using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Observer
    {
        public Observer()
        {
            Notes = new HashSet<Note>();
            Answers = new HashSet<Answer>();
            PollingStationInfos = new HashSet<PollingStationInfo>();
        }

        public int Id { get; set; }
        public bool FromTeam { get; set; }
        public int IdNgo { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public string Pin { get; set; }
        public string MobileDeviceId { get; set; }
        public DateTime? DeviceRegisterDate { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<PollingStationInfo> PollingStationInfos { get; set; }
        public virtual Ngo Ngo { get; set; }
    }
}
