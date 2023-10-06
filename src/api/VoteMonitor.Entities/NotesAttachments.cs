using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VoteMonitor.Entities;

public class NotesAttachments : IIdentifiableEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public int NoteId { get; set; }
    public virtual Note Note { get; set; }
    public string Path { get; set; }
    public string FileName { get; set; }
}
