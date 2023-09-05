namespace VoteMonitor.Api.County.Models;

public class CountyModel
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int NumberOfPollingStations { get; set; }
    public bool Diaspora { get; set; }
    public int Order { get; set; }
}