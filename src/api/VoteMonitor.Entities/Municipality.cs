namespace VoteMonitor.Entities;

public class Municipality : IIdentifiableEntity
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }
    public int CountyId { get; set; }
    public County County { get; set; }

    public virtual ICollection<PollingStation> PollingStations { get; set; } = new HashSet<PollingStation>();
}
