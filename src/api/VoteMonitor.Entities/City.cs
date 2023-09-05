namespace VoteMonitor.Entities;

public class City : IIdentifiableEntity
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<PollingStation> PollingStations { get; set; } = new HashSet<PollingStation>();
}
