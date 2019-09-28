using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Entities
{
    public class NoteAttachment
    {
        [Key, Required]
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string NotePath { get; set; }

        public int NoteId { get; set; }
        public Note Note { get; set; }
    }
}
