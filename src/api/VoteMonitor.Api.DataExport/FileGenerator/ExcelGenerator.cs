using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;

namespace VoteMonitor.Api.DataExport.FileGenerator
{
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
                {
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                }

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
                    else if (_type[j].ToLower() == ExcelUtility.STRING)
                    {
                        Row1.SetCellValue(cellvalue);
                    }
                    else if (_type[j].ToLower() == ExcelUtility.INT32)
                    {
                        Row1.SetCellValue(Convert.ToInt32(table.Rows[i][j]));
                    }
                    else if (_type[j].ToLower() == ExcelUtility.DOUBLE)
                    {
                        Row1.SetCellValue(Convert.ToDouble(table.Rows[i][j]));
                    }
                    else if (_type[j].ToLower() == ExcelUtility.DATETIME)
                    {
                        Row1.SetCellValue(Convert.ToDateTime
                             (table.Rows[i][j]).ToString(ExcelUtility.DATETIME_FORMAT));
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
}