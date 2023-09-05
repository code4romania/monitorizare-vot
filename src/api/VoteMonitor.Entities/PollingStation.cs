namespace VoteMonitor.Entities;

public class PollingStation : IIdentifiableEntity
{
    public int Id { get; set; }
    public int IdCounty { get; set; }
    public int Number { get; set; }
    public string Address { get; set; }

    public ICollection<Note> Notes { get; set; } = new HashSet<Note>();
    public ICollection<Answer> Answers { get; set; } = new HashSet<Answer>();
    public ICollection<PollingStationInfo> PollingStationInfos { get; set; } = new HashSet<PollingStationInfo>();
    public County County { get; set; }
}
