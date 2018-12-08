using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class VotingSection
    {
        public VotingSection()
        {
            Ratings = new HashSet<Rating>();
            Answers = new HashSet<Answer>();
            FormAnswers = new HashSet<FormAnswer>();
        }

        public int VotingSectionId { get; set; }
        public string SectionAdress { get; set; }
        public string Coordinate { get; set; }
        public string UatName { get; set; }
        public int CountyId { get; set; }
        public string ComponentPlace { get; set; }
        public int SectionNumber { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
        public virtual ICollection<FormAnswer> FormAnswers { get; set; }
        public virtual County CountyNavigationId { get; set; }
    }
}
