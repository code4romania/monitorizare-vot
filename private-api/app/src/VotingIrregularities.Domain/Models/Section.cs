using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Section
    {
        public Section()
        {
            Questions = new HashSet<Question>();
        }

        public int SectionId { get; set; }
        public string SectionCode { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
    }
}
