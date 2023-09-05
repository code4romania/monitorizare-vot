using Microsoft.Extensions.Caching.Distributed;

namespace VoteMonitor.Api.Core.Services;

public class NoCacheService : ICacheService
{
    public async Task<T> GetOrSaveDataInCacheAsync<T>(string name, Func<Task<T>> source,
        DistributedCacheEntryOptions options = null)
    {
        return await source();
    }

    public Task<T> GetObjectSafeAsync<T>(string name) => throw new NotImplementedException();

    public Task SaveObjectSafeAsync(string name, object value,
        DistributedCacheEntryOptions options = null) => throw new NotImplementedException();
}