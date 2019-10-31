using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Entities
{
    public class NoteAttachment
    {
        public NoteAttachment(string notePath)
            => NotePath = notePath;

        [Key, Required]
        public int Id { get; set; }

        [Required, MaxLength(1000)]
        public string NotePath { get; set; }

        public int NoteId { get; set; }
        public virtual Note Note { get; set; }
    }
}
