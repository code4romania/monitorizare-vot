using System;

namespace VoteMonitor.Entities
{
    public class ExportModel
    {
        public Guid Id { get; set; }
        public string ObserverPhone { get; set; }
        public int IdNgo { get; set; }
        public string FormCode { get; set; }
        public string QuestionText { get; set; }
        public string OptionText { get; set; }
        public string AnswerFreeText { get; set; }
        public string NoteText { get; set; }
        public string NoteAttachmentPath { get; set; }
        public DateTime LastModified { get; set; }
        public string CountyCode { get; set; }
        public int PollingStationNumber { get; set; }
    }
}