using CsvHelper;
using System.Globalization;

namespace VoteMonitor.Api.DataExport.FileGenerator;

public class CsvGenerator : ICsvGenerator
{
    public byte[] Export<T>(IEnumerable<T> exportData, string fileName)
    {
        using (var mem = new MemoryStream())
        using (var writer = new StreamWriter(mem))
        using (var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(exportData);

            writer.Flush();
            return mem.ToArray();
        }
    }
}
