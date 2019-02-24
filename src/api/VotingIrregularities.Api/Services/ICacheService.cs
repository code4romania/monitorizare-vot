using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace VotingIrregularities.Api.Services
{
    /// <summary>
    /// Interface for the caching service to be used.
    /// </summary>
    public interface ICacheService
    {
        Task<T> GetOrSaveDataInCacheAsync<T>(CacheObjectsName name, Func<Task<T>> source, DistributedCacheEntryOptions options = null);
        Task<T> GetObjectSafeAsync<T>(CacheObjectsName name);
        Task SaveObjectSafeAsync(CacheObjectsName name, object value, DistributedCacheEntryOptions options = null);

    }
}
