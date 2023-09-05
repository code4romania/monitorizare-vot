using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities;

public class Question : IHierarchicalEntity<OptionToQuestion>, IIdentifiableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Code { get; set; }
    public int IdSection { get; set; }
    public QuestionType QuestionType { get; set; }
    [Required, MaxLength(1000)]
    public string Text { get; set; }
    public string Hint { get; set; }
    public int OrderNumber { get; set; }

    public ICollection<Note> Notes { get; set; } = new HashSet<Note>();
    public ICollection<OptionToQuestion> OptionsToQuestions { get; set; } = new HashSet<OptionToQuestion>();
    public FormSection FormSection { get; set; }

    ICollection<OptionToQuestion> IHierarchicalEntity<OptionToQuestion>.Children { get => OptionsToQuestions; set => OptionsToQuestions = value; }
}
