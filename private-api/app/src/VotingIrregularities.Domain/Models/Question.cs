using System.Collections.Generic;
using VotingIrregularities.Domain.ValueObjects;

namespace VotingIrregularities.Domain.Models
{
    public partial class Question
    {
        public Question()
        {
            Notes = new HashSet<Note>();
            OptionsToQuestions = new HashSet<OptionToQuestion>();
        }

        public int Id { get; set; }
        public string FormCode { get; set; }
        public string Code { get; set; }
        public int IdSection { get; set; }
        public QuestionType QuestionType { get; set; }
        public string Text { get; set; }
        public string Hint { get; set; }

        public virtual ICollection<Note> Notes { get; set; }
        public virtual ICollection<OptionToQuestion> OptionsToQuestions { get; set; }
        public virtual FormSection FormSection { get; set; }
    }
}
