using Microsoft.Extensions.Caching.Distributed;

namespace VoteMonitor.Api.Core.Services;

/// <summary>
/// Interface for the caching service to be used.
/// </summary>
public interface ICacheService
{
    Task<T> GetOrSaveDataInCacheAsync<T>(string name, Func<Task<T>> source, DistributedCacheEntryOptions options = null);
    Task<T> GetObjectSafeAsync<T>(string name);
    Task SaveObjectSafeAsync<T>(string name, T value, DistributedCacheEntryOptions options = null);
    Task ClearAllValuesAsync();

    Task RemoveValueAsync(string cacheKey);
}
