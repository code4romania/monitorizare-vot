namespace VoteMonitor.Api.DataExport.FileGenerator;

public interface ICsvGenerator
{
    byte[] Export<T>(IEnumerable<T> exportData, string fileName);
}