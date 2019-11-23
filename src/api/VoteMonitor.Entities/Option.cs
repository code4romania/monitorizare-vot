using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities
{
    public partial class Option
    {
        public Option()
        {
            OptionsToQuestions = new HashSet<OptionToQuestion>();
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public bool IsFreeText { get; set; }
        [Required, MaxLength(1000)]
        public string Text { get; set; }
        public string Hint { get; set; }

        public virtual ICollection<OptionToQuestion> OptionsToQuestions { get; set; }
    }
}
