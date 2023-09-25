namespace VoteMonitor.Api.County.Models;

public class MunicipalityModel
{
    public int Id { get; set; }
    public string CountyCode { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int NumberOfPollingStations { get; set; }
    public int Order { get; set; }
}
