namespace VoteMonitor.Entities;

public class County : IIdentifiableEntity
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public bool Diaspora { get; set; }
    public int Order { get; set; }

    public virtual ICollection<Municipality> Municipalities { get; set; } = new HashSet<Municipality>();
}
