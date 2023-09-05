using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Collections;
using System.ComponentModel;

namespace VoteMonitor.Api.DataExport.FileGenerator;

public class ExcelFile
{
    private readonly IWorkbook _workbook;
    private readonly ICellStyle _headerStyle;


    private ExcelFile()
    {
        _workbook = new XSSFWorkbook();

        _headerStyle = _workbook.CreateCellStyle();
        var headerFont = _workbook.CreateFont();
        headerFont.IsBold = true;
        _headerStyle.SetFont(headerFont);
    }

    public static ExcelFile New()
    {
        return new ExcelFile();
    }

    public ExcelFile WithSheet<T>(string sheetName, List<T> exportData)
    {
        var sheet = _workbook.CreateSheet(sheetName);

        PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));

        #region Generating SheetRow based on datatype

        Dictionary<int, string> headers = new Dictionary<int, string>();

        for (int i = 0; i < exportData.Count; i++)
        {
            var row = sheet.CreateRow(i + 1);
            int columnIndex = 0;
            T item = exportData[i];
            WriteValues(item, headers, properties, row, ref columnIndex);
        }
        #endregion

        #region Generating Header Cells
        var header = sheet.CreateRow(0);
        for (var i = 0; i < headers.Count; i++)
        {
            var cell = header.CreateCell(i);
            cell.SetCellValue(headers[i]);
            cell.CellStyle = _headerStyle;
            // It's heavy, it slows down your Excel if you have large data
            sheet.AutoSizeColumn(i);
        }
        #endregion

        return this;
    }

    private static void WriteValues<T>(T item,
        Dictionary<int, string> headers,
        PropertyDescriptorCollection properties,
        IRow row,
        ref int columnIndex,
        string columnNamePrefix = "")
    {
        foreach (PropertyDescriptor prop in properties)
        {
            ICell cell = row.CreateCell(columnIndex);

            var columnName = $"{columnNamePrefix}{prop.Name}";
            if (prop.PropertyType == typeof(string))
            {
                cell.SetCellValue(prop.GetValue(item) as string);
                headers.TryAdd(columnIndex, columnName);
            }

            if (prop.PropertyType == typeof(int))
            {
                cell.SetCellValue((int)prop.GetValue(item));
                headers.TryAdd(columnIndex, columnName);
            }

            if (prop.PropertyType == typeof(double))
            {
                cell.SetCellValue((double)prop.GetValue(item));
                headers.TryAdd(columnIndex, columnName);
            }

            if (prop.PropertyType == typeof(DateTime))
            {
                cell.SetCellValue(((DateTime)prop.GetValue(item)).ToString(ExcelUtility.DATETIME_FORMAT));
                headers.TryAdd(columnIndex, columnName);
            }

            if (prop.PropertyType == typeof(bool))
            {
                cell.SetCellValue((bool)prop.GetValue(item));
                headers.TryAdd(columnIndex, columnName);
            }

            if (prop.PropertyType.IsEnum)
            {
                cell.SetCellValue(Convert.ToString(prop.GetValue(item)));
                headers.TryAdd(columnIndex, columnName);
            }

            if (prop.PropertyType.IsGenericType &&
                prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
            {
                var type = prop.PropertyType.GetGenericArguments()[0];
                var childItems = prop.GetValue(item) as IList;

                for (var index = 0; index < childItems.Count; index++)
                {
                    var childItem = childItems[index];
                    WriteValues(childItem, headers, TypeDescriptor.GetProperties(type), row, ref columnIndex, $"{prop.Name}[{index}]_");
                }
            }

            columnIndex++;
        }
    }

    public byte[] Write()
    {
        using (var memoryStream = new MemoryStream())
        {
            _workbook.Write(memoryStream);

            return memoryStream.ToArray();
        }
    }
}