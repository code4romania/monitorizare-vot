using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Question
    {
        public Question()
        {
            Ratings = new HashSet<Rating>();
            AvailableAnswer = new HashSet<AvailableAnswer>();
        }

        public int QuestionId { get; set; }
        public string FormCode { get; set; }
        public string QuestionCode { get; set; }
        public int SectionId { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionText { get; set; }

        public virtual ICollection<Rating> Ratings { get; set; }
        public virtual ICollection<AvailableAnswer> AvailableAnswer { get; set; }
        public virtual Section NavigationSectionId { get; set; }
    }
}
