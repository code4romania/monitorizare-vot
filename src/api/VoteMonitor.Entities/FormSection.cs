using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities;

public class FormSection : IHierarchicalEntity<Question>, IIdentifiableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public int IdForm { get; set; }

    public int OrderNumber { get; set; }

    public Form Form { get; set; }

    public ICollection<Question> Questions { get; set; } = new HashSet<Question>();

    ICollection<Question> IHierarchicalEntity<Question>.Children { get => Questions; set => Questions = value; }
}
