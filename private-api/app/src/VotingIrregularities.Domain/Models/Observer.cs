using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Observer
    {
        public Observer()
        {
            Ratings = new HashSet<Rating>();
            Answers = new HashSet<Answer>();
            FormAnswers = new HashSet<FormAnswer>();
        }

        public int ObserverId { get; set; }
        public bool IsPartOfTheTeam { get; set; }
        public int NgoId { get; set; }
        public string TelephoneNumber { get; set; }
        public string Fullname { get; set; }
        public string Pin { get; set; }
        public string MobileDeviceId { get; set; }
        public DateTime? DeviceRegistrationDate { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<FormAnswer> FormAnswers { get; set; }
        public virtual Ngo NgoNavigationId { get; set; }
    }
}
