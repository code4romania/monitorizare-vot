using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class OptionToQuestion
    {
        public OptionToQuestion()
        {
            Answers = new HashSet<Answer>();
        }

        public int Id { get; set; }
        public int IdQuestion { get; set; }
        public int IdOption { get; set; }
        public bool Flagged { get; set; }

        public virtual ICollection<Answer> Answers { get; }
        public virtual Question Question { get; set; }
        public virtual Option Option { get; set; }
    }
}
