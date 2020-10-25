using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities
{
    public partial class OptionToQuestion : IIdentifiableEntity
    {
        public OptionToQuestion()
        {
            Answers = new HashSet<Answer>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int IdQuestion { get; set; }
        public int IdOption { get; set; }
        public bool Flagged { get; set; }

        public virtual ICollection<Answer> Answers { get; }
        public virtual Question Question { get; set; }
        public virtual Option Option { get; set; }
    }
}
