using System.Linq;

namespace VoteMonitor.Api.Core.Extensions
{
    public class QueryExtension
    {
        public IQueryable<T> GetPagedQuery<T>(IQueryable<T> entities, int page, int pageSize)
            where T: class // could be more restrictive if all entities implemented the same interface or the same base class
        {
            if (pageSize > 0)
            {
                return entities
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize);
            }

            return entities;
        }
    }
}
