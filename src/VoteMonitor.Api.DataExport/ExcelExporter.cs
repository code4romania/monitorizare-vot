using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace VoteMonitor.Api.DataExport
{
    public interface IExcelGenerator
    {
        byte[] Export<T>(List<T> exportData, string fileName,
            bool appendDateTimeInFileName = false, string sheetName = Utility.DEFAULT_SHEET_NAME);
    }

    public class Utility
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
    public class ExcelGenerator : ExcelGeneratorBase
    {
        public ExcelGenerator()
        {
            _headers = new List<string>();
            _type = new List<string>();
        }

        public sealed override void WriteData<T>(List<T> exportData)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            #region Reading property name to generate cell header
            foreach (PropertyDescriptor prop in properties)
            {
                var type = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                _type.Add(type.Name);
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
                string name = Regex.Replace(prop.Name, "([A-Z])", " $1").Trim(); //space seperated name by caps for header
                _headers.Add(name);
            }
            #endregion

            #region Generating Datatable from List
            foreach (T item in exportData)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            #endregion

            #region Generating SheetRow based on datatype
            IRow sheetRow = null;

            for (int i = 0; i < table.Rows.Count; i++)
            {
                sheetRow = _sheet.CreateRow(i + 1);
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    // TODO: Below commented code is for Wrapping and Alignment of cell
                    // Row1.CellStyle = CellCentertTopAlignment;
                    // Row1.CellStyle.WrapText = true;
                    // ICellStyle CellCentertTopAlignment = _workbook.CreateCellStyle();
                    // CellCentertTopAlignment = _workbook.CreateCellStyle();
                    // CellCentertTopAlignment.Alignment = HorizontalAlignment.Center;

                    ICell Row1 = sheetRow.CreateCell(j);
                    string cellvalue = Convert.ToString(table.Rows[i][j]);

                    // TODO: move it to switch case

                    if (string.IsNullOrWhiteSpace(cellvalue))
                    {
                        Row1.SetCellValue(string.Empty);
                    }
                    else if (_type[j].ToLower() == Utility.STRING)
                    {
                        Row1.SetCellValue(cellvalue);
                    }
                    else if (_type[j].ToLower() == Utility.INT32)
                    {
                        Row1.SetCellValue(Convert.ToInt32(table.Rows[i][j]));
                    }
                    else if (_type[j].ToLower() == Utility.DOUBLE)
                    {
                        Row1.SetCellValue(Convert.ToDouble(table.Rows[i][j]));
                    }
                    else if (_type[j].ToLower() == Utility.DATETIME)
                    {
                        Row1.SetCellValue(Convert.ToDateTime
                             (table.Rows[i][j]).ToString(Utility.DATETIME_FORMAT));
                    }
                    else
                    {
                        Row1.SetCellValue(string.Empty);
                    }
                }
            }
            #endregion
        }
    }
    public abstract class ExcelGeneratorBase : IExcelGenerator
    {
        protected string _sheetName;
        protected string _fileName;
        protected List<string> _headers;
        protected List<string> _type;
        protected IWorkbook _workbook;
        protected ISheet _sheet;

        /// <summary>
        /// Common Code for the Export
        /// It creates Workbook, Sheet, Generate Header Cells and returns HttpResponseMessage
        /// </summary>
        /// <typeparam name="T">Generic Class Type</typeparam>
        /// <param name="exportData">Data to be exported</param>
        /// <param name="fileName">Export File Name</param>
        /// <param name="appendDateTimeInFileName">Specify if filename should contain when file was generated</param>
        /// <param name="sheetName">First Sheet Name</param>
        /// <returns></returns>
        public byte[] Export<T>(List<T> exportData, string fileName,
            bool appendDateTimeInFileName = false,
            string sheetName = Utility.DEFAULT_SHEET_NAME)
        {
            _sheetName = sheetName;

            _fileName = appendDateTimeInFileName
                ? $"{fileName}_{DateTime.Now.ToString(Utility.DEFAULT_FILE_DATETIME)}"
                : fileName;

            #region Generation of Workbook, Sheet and General Configuration
            _workbook = new XSSFWorkbook();
            _sheet = _workbook.CreateSheet(_sheetName);

            var headerStyle = _workbook.CreateCellStyle();
            var headerFont = _workbook.CreateFont();
            headerFont.IsBold = true;
            headerStyle.SetFont(headerFont);
            #endregion

            WriteData(exportData);

            #region Generating Header Cells
            var header = _sheet.CreateRow(0);
            for (var i = 0; i < _headers.Count; i++)
            {
                var cell = header.CreateCell(i);
                cell.SetCellValue(_headers[i]);
                cell.CellStyle = headerStyle;
                // It's heavy, it slows down your Excel if you have large data                
                _sheet.AutoSizeColumn(i);
            }
            #endregion

            #region Generating and Returning Stream for Excel
            using (var memoryStream = new MemoryStream())
            {
                _workbook.Write(memoryStream);

                return memoryStream.ToArray();
            }
            #endregion
        }

        /// <summary>
        /// Generic Definition to handle all types of List
        /// Overrride this function to provide your own implementation
        /// </summary>
        /// <param name="exportData"></param>
        public abstract void WriteData<T>(List<T> exportData);
    }
}