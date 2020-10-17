using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Logging;
using System.IO;
using CsvHelper;

namespace VoteMonitor.Api.DataExport
{
    public interface ICsvGenerator
    {
        byte[] Export<T>(IEnumerable<T> exportData, string fileName);
    }

    public partial class Utility
    {
        public const string CSV_MEDIA_TYPE = "text/csv"; // ref: https://stackoverflow.com/questions/7076042/what-mime-type-should-i-use-for-csv > https://tools.ietf.org/html/rfc7111
    }

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