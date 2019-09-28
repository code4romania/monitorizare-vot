using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VoteMonitor.Api.Note.Models
{
    public class NoteModel
    {
        [Required(AllowEmptyStrings = false)]
        public string CountyCode { get; set; }
        [Required]
        public int PollingStattionNumber { get; set; }
        public int? QuestionId { get; set; }
        public string Text { get; set; }
        public List<string> NoteAttachments { get; set; }
        public string FormCode { get; set; }
    }
}
