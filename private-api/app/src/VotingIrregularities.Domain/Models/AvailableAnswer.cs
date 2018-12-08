using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class AvailableAnswer
    {
        public AvailableAnswer()
        {
            Answers = new HashSet<Answer>();
        }

        public int AvailableAnswerId { get; set; }
        public int QuestionId { get; set; }
        public int OptionId { get; set; }
        public bool AnswerWithFlag { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
        public virtual Question QuestionNavigationId { get; set; }
        public virtual Option OptionNavigationId { get; set; }
    }
}
