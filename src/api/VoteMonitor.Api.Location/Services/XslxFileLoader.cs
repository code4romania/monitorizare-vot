using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VoteMonitor.Api.Location.Models;

namespace VoteMonitor.Api.Location.Services
{
    public class XlsxFileLoader : IFileLoader
    {
        private static readonly string[] SUFFIXES = { ".xlsx", ".xls" };
        private static PollingStationExcelHeader header = new PollingStationExcelHeader();

        public byte[] ExportHeaderInformation()
        {
            using var excelPackage = new ExcelPackage();
            var workSheet = excelPackage.Workbook.Worksheets.Add("HeaderInfo");

            var maximumColumnIndex = header.Columns.Max(x => x.Index);

            var headerColumns = new List<string>();

            for (int i = 1; i <= maximumColumnIndex; i++)
            {
                workSheet.Cells[1, i].Value = header.Columns.FirstOrDefault(x => x.Index == i)?.Name ?? "not used";
            }

            return excelPackage.GetAsByteArray();
        }

        public async Task<List<PollingStationDTO>> ImportFileAsync(IFormFile file)
        {
            if (file == null || file.Length <= 0)
            {
                throw new ArgumentException();
            }

            List<PollingStationDTO> resultList = new List<PollingStationDTO>();

            using (var memoryStream = new MemoryStream())
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken cancellationToken = source.Token;

                await file.CopyToAsync(memoryStream, cancellationToken);

                using (var excelPackage = new ExcelPackage(memoryStream))
                {
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                    for (int index = 2; index <= workSheet.Dimension.Rows; ++index)
                    {
                        if (workSheet.Cells[index, header[nameof(PollingStationDTO.NrSV)].Index].Text.Length == 0)
                        {
                            continue;
                        }
                        PollingStationDTO dto = new PollingStationDTO();
                        dto.Adresa = workSheet.Cells[index, header[nameof(PollingStationDTO.Adresa)].Index].Text;
                        dto.CodJudet = workSheet.Cells[index, header[nameof(PollingStationDTO.CodJudet)].Index].Text;
                        Int32.TryParse(workSheet.Cells[index, header[nameof(PollingStationDTO.CodSiruta)].Index].Text, out int codValue);
                        Int32.TryParse(workSheet.Cells[index, header[nameof(PollingStationDTO.NrSV)].Index].Text, out int nrSectieValue);
                        dto.CodSiruta = codValue;
                        dto.NrSV = nrSectieValue;
                        resultList.Add(dto);
                    }
                }
            }

            return resultList;
        }

        public bool ValidateFile(IFormFile file)
        {
            foreach (string Suffix in SUFFIXES)
            {
                if (Path.GetExtension(file.FileName).EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}