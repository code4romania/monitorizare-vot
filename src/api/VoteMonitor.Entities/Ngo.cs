namespace VoteMonitor.Entities;

public class Ngo : IIdentifiableEntity
{
    public int Id { get; set; }
    public string ShortName { get; set; }
    public string Name { get; set; }
    public bool Organizer { get; set; }
    public bool IsActive { get; set; }

    public ICollection<NgoAdmin> NgoAdmins { get; set; } = new HashSet<NgoAdmin>();
    public ICollection<Observer> Observers { get; set; } = new HashSet<Observer>();
}
