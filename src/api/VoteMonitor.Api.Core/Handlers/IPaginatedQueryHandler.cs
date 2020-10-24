using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoteMonitor.Api.Core.Handlers
{
    public interface IPaginatedQueryHandler
    { }

    public static class PaginatedQueryHandlerExtension
    {
        public static IQueryable<T> GetPagedQuery<T>(this IPaginatedQueryHandler _, IQueryable<T> entities, int page, int pageSize)
            where T: class // clud be more restrictive if all entities implements the same interface of base class
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
