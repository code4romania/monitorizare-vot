namespace VoteMonitor.Api.DataExport.FileGenerator
{
    public class ExcelUtility
    {
        public const string DATETIME_FORMAT = "dd/MM/yyyy hh:mm:ss";
        public const string EXCEL_MEDIA_TYPE = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";


        #region DataType available for Excel Export
        public const string STRING = "string";
        public const string INT32 = "int32";
        public const string DOUBLE = "double";
        public const string DATETIME = "datetime";
        public const string BOOLEAN = "boolean";
        #endregion
    }
}
