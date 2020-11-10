using System;

namespace VoteMonitor.Api.Note.Models
{
    [Obsolete("Will be removed when ui will use multiple files upload")]
    public class UploadNoteResult
    {
        public string FileAddress { get; set; }
        public UploadNoteModel Note { get; set; }
    }
}
