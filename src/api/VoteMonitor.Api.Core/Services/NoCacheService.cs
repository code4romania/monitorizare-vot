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

    public Task SaveObjectSafeAsync<T>(string name, T value,
        DistributedCacheEntryOptions options = null) => throw new NotImplementedException();

    public Task RemoveValueAsync(string cacheKey) => Task.CompletedTask;

    public Task ClearAllValuesAsync() => Task.CompletedTask;
}
