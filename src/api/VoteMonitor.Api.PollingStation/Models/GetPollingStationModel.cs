namespace VoteMonitor.Api.PollingStation.Models;

public class GetPollingStationModel
{
    public int Id { get; set; }
    public int IdCounty { get; set; }
    public string Address { get; set; }
    public int Number { get; set; }
}