using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace VotingIrregularities.Api.Services
{
    public interface ICacheService
    {
        Task<T> GetOrSaveDataInCache<T>(CacheObjectsName name, Func<T> source, DistributedCacheEntryOptions options = null);
        Task<T> GetObjectSafeAsync<T>(CacheObjectsName name);
        Task SaveObjectSafeAsync(CacheObjectsName name, object value, DistributedCacheEntryOptions options = null);

    }
}
