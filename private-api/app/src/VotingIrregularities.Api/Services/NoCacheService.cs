using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace VotingIrregularities.Api.Services
{
    public class NoCacheService : ICacheService
    {
        public async Task<T> GetOrSaveDataInCacheAsync<T>(CacheObjectsName name, Func<Task<T>> source,
            DistributedCacheEntryOptions options = null)
        {
            return await source();
        }

        public async Task<T> GetObjectSafeAsync<T>(CacheObjectsName name)
        {
            throw new NotImplementedException();
        }

        public async Task SaveObjectSafeAsync(CacheObjectsName name, object value,
            DistributedCacheEntryOptions options = null)
        {
            throw new NotImplementedException();
        }
    }
}
