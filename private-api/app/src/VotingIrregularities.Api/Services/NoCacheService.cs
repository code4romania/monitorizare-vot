using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
