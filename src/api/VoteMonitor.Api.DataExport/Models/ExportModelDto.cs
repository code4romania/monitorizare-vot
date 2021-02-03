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
        public DateTime LastModified { get; set; }
        public string CountyCode { get; set; }
        public int PollingStationNumber { get; set; }
        public bool HasNotes => NumberOfNotes > 0;
        public int NumberOfNotes { get; set; }
        public bool HasAttachments => NumberOfAttachments > 0;
        public int NumberOfAttachments { get; set; }
    }    
}
