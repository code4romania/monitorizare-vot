using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities;

public class OptionToQuestion : IIdentifiableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public int IdQuestion { get; set; }
    public int IdOption { get; set; }
    public bool Flagged { get; set; }

    public virtual ICollection<Answer> Answers { get; } = new HashSet<Answer>();
    public virtual ICollection<AnswerCorrupted> CorruptedAnswers { get; } = new HashSet<AnswerCorrupted>();
    public virtual Question Question { get; set; }
    public virtual Option Option { get; set; }
}
