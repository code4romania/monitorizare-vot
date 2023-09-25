namespace VoteMonitor.Api.County.Models;

public class MunicipalityModelV2
{
    public int Id { get; set; }
    
    public string Code { get; set; }
    public string Name { get; set; }
    public int NumberOfPollingStations { get; set; }
    public int Order { get; set; }

    public int CountyId { get; set; }
    public string CountyCode { get; set; }
    public string CountyName { get; set; }
    public bool Diaspora { get; set; }
    public int CountyOrder { get; set; }
}
