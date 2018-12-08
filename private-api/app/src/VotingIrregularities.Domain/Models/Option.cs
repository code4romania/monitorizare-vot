using System;
using System.Collections.Generic;

namespace VotingIrregularities.Domain.Models
{
    public partial class Option
    {
        public Option()
        {
            AvailableAnswers = new HashSet<AvailableAnswer>();
        }

        public int OptionId { get; set; }
        public bool TextMustBeInserted { get; set; }
        public string TextOption { get; set; }

        public virtual ICollection<AvailableAnswer> AvailableAnswers { get; set; }
    }
}
