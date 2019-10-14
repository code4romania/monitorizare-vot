using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using MonitorizareVot.Api.Location.Models;
using OfficeOpenXml;

namespace MonitorizareVot.Api.Location.Services
{
    public class XlsxFileLoader : IFileLoader
    {
        private static readonly string[] SUFFIXES = {".xlsx", ".xls"};
        private static readonly int COD_JUDET_INDEX = 1;
        private static readonly int ADRESA_INDEX = 7;
        private static readonly int SIRUTA_INDEX = 3;
        private static readonly int NR_SECTIE_INDEX = 5;

        public async Task<List<PollingStationDTO>> ImportFileAsync(IFormFile file)
        {
            if(file == null || file.Length <= 0)
                throw new System.ArgumentException();

            List<PollingStationDTO> resultList = new List<PollingStationDTO>();

            using(var memoryStream = new MemoryStream())
            {
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken cancellationToken = source.Token;

                await file.CopyToAsync(memoryStream, cancellationToken);

                using(var excelPackage = new ExcelPackage(memoryStream))
                {
                    ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[0];
                    for(int index = 2; index < workSheet.Dimension.Rows; ++index)
                    {
                        if(workSheet.Cells[index, NR_SECTIE_INDEX].Text.Length == 0)
                            continue;

                        PollingStationDTO dto = new PollingStationDTO();
                        dto.Adresa = workSheet.Cells[index, ADRESA_INDEX].Text;
                        dto.CodJudet = workSheet.Cells[index, COD_JUDET_INDEX].Text;
                        Int32.TryParse(workSheet.Cells[index, SIRUTA_INDEX].Text, out int codValue);
                        Int32.TryParse(workSheet.Cells[index, NR_SECTIE_INDEX].Text, out int nrSectieValue);
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
            foreach(string Suffix in SUFFIXES)
            {
                if(Path.GetExtension(file.FileName).EndsWith(Suffix, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
} 