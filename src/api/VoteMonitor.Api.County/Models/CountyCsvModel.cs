using CsvHelper.Configuration.Attributes;

namespace VoteMonitor.Api.County.Models;

public class CountyCsvModel
{
    [Index(0)]
    public int Id { get; set; }

    [Index(1)]
    public string Code { set; get; }

    [Index(2)]
    public string Name { get; set; }

    [Index(3)]
    public bool Diaspora { get; set; }

    [Index(4)]
    public int Order { get; set; }
}