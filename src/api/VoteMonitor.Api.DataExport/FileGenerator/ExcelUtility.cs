namespace VoteMonitor.Api.DataExport.FileGenerator
{
    public class ExcelUtility
    {
        public const string DEFAULT_SHEET_NAME = "Sheet1";
        public const string DEFAULT_FILE_DATETIME = "yyyyMMdd_HHmm";
        public const string DATETIME_FORMAT = "dd/MM/yyyy hh:mm:ss";
        public const string EXCEL_MEDIA_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        public const string DISPOSITION_TYPE_ATTACHMENT = "attachment";


        #region DataType available for Excel Export
        public const string STRING = "string";
        public const string INT32 = "int32";
        public const string DOUBLE = "double";
        public const string DATETIME = "datetime";
        #endregion
    }
}
