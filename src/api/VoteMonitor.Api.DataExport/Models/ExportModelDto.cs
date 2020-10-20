using System;

namespace VoteMonitor.Api.DataExport.Models
{
    /// <summary>
    /// data transfer object for ExportModel on DataExportQueryHandler
    /// Based on ExportModel entity, hide not necessary Id field
    /// </summary>
    public class ExportModelDto
    {
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
