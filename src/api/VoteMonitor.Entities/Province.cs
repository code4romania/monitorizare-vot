namespace VoteMonitor.Entities;

public class Province : IIdentifiableEntity
{
    public int Id { get; set; }

    public string Code { get; set; }
    public string Name { get; set; }
    public int Order { get; set; }

    public virtual ICollection<County> Counties { get; set; } = new HashSet<County>();
}
