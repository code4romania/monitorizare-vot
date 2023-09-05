using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities;

public class Option : IIdentifiableEntity
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public bool IsFreeText { get; set; }
    [Required, MaxLength(1000)]
    public string Text { get; set; }
    public string Hint { get; set; }
    public int OrderNumber { get; set; }

    public virtual ICollection<OptionToQuestion> OptionsToQuestions { get; set; } = new HashSet<OptionToQuestion>();
}
