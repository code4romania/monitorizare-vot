using System.Collections.Generic;

namespace VoteMonitor.Api.DataExport.FileGenerator
{
    public interface IExcelGenerator
    {
        byte[] Export<T>(List<T> exportData, string fileName,
            bool appendDateTimeInFileName = false, string sheetName = ExcelUtility.DEFAULT_SHEET_NAME);
    }
}
