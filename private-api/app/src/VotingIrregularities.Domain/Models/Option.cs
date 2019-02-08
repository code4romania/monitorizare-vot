using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Option
    {
        public Option()
        {
            OptionsToQuestions = new HashSet<OptionToQuestion>();
        }

        public int Id { get; set; }
        public bool IsFreeText { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }

        public virtual ICollection<OptionToQuestion> OptionsToQuestions { get; set; }
    }
}
