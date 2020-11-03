using CsvHelper;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace VoteMonitor.Api.DataExport.FileGenerator
{
    public class CsvGenerator : ICsvGenerator
    {
        private readonly ILogger _logger;

        public CsvGenerator(ILogger logger)
        {
            _logger = logger;
        }

        public byte[] Export<T>(IEnumerable<T> exportData, string fileName)
        {
            if (exportData == null || !exportData.Any())
                return new byte[0];


            using (var mem = new MemoryStream())
            using (var writer = new StreamWriter(mem))
            using (var csvWriter = new CsvWriter(writer))
            {
                csvWriter.Configuration.HasHeaderRecord = true;
                csvWriter.Configuration.AutoMap<T>();

                csvWriter.WriteRecords(exportData);

                writer.Flush();
                return mem.ToArray();
            }
        }
    }
}