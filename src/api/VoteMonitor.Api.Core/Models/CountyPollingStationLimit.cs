namespace VoteMonitor.Api.Core.Models;

public class CountyPollingStationLimit
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public bool Diaspora { get; set; }
    public int Order { get; set; }
}
