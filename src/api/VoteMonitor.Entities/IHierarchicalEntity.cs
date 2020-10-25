using System.Collections.Generic;

namespace VoteMonitor.Entities
{
    public interface IHierarchicalEntity<TChildEntity>
    {
        ICollection<TChildEntity> Children { get; set; }
    }
}
