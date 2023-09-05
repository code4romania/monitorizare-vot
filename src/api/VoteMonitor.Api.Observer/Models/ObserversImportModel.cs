using CsvHelper.Configuration.Attributes;

namespace VoteMonitor.Api.Observer.Models;

internal class ObserversImportModel
{
    [Index(0)]
    public string Phone { get; set; }

    [Index(1)]
    public string Pin { get; set; }

    [Index(2)]
    public string Name { get; set; }
}