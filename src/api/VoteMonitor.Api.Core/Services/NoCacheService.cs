using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace VoteMonitor.Api.Core.Services
{
    public class NoCacheService : ICacheService
    {
        public async Task<T> GetOrSaveDataInCacheAsync<T>(CacheObjectsName name, Func<Task<T>> source,
            DistributedCacheEntryOptions options = null)
        {
            return await source();
        }

        public Task<T> GetObjectSafeAsync<T>(CacheObjectsName name) => throw new NotImplementedException();

        public Task SaveObjectSafeAsync(CacheObjectsName name, object value,
            DistributedCacheEntryOptions options = null) => throw new NotImplementedException();
    }
}
