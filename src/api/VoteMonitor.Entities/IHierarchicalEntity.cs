namespace VoteMonitor.Entities;

public interface IHierarchicalEntity<TChildEntity>
{
    ICollection<TChildEntity> Children { get; set; }
}