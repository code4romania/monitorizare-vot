using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace VoteMonitor.Api.Core.Services;

/// <inheritdoc />
public class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ILogger _logger;
    private readonly ConcurrentDictionary<string, string> _cacheKeys = new();
    public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T> GetOrSaveDataInCacheAsync<T>(string name, Func<Task<T>> source,
        DistributedCacheEntryOptions options = null)
    {
        var obj = await GetObjectSafeAsync<T>(name);

        if (obj != null)
        {
            return obj;
        }

        var result = await source();

        await SaveObjectSafeAsync(name, result, options);

        return result;
    }

    public async Task<T> GetObjectSafeAsync<T>(string name)
    {
        var result = default(T);

        try
        {
            var cache = await _cache.GetAsync(name);

            if (cache == null)
            {
                _logger.LogInformation($"Cache missed for {name}");
                return default(T);
            }

            var obj = JsonConvert.DeserializeObject<T>(GetString(cache));

            return obj;

        }
        catch (Exception exception)
        {
            _logger.LogError(GetHashCode(), exception, exception.Message);
        }

        return result;
    }

    public async Task SaveObjectSafeAsync<T>(string name, T value, DistributedCacheEntryOptions options = null)
    {
        try
        {
            var obj = JsonConvert.SerializeObject(value);

            _cacheKeys.TryAdd(name, string.Empty);

            if (options != null)
            {
                await _cache.SetAsync(name, GetBytes(obj), options);
            }
            else
            {
                await _cache.SetAsync(name, GetBytes(obj));
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(GetHashCode(), exception, exception.Message);
        }
    }

    public async Task RemoveValueAsync(string cacheKey)
    {
        await _cache.RemoveAsync(cacheKey);
    }

    public async Task ClearAllValuesAsync()
    {
        var tasks = new List<Task>();
        foreach (var cacheKey in _cacheKeys)
        {
            tasks.Add(_cache.RemoveAsync(cacheKey.Key));
        }

        await Task.WhenAll(tasks);
    }

    private static byte[] GetBytes(string str)
    {
        var bytes = new byte[str.Length * sizeof(char)];
        Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
        return bytes;
    }

    private static string GetString(byte[] bytes)
    {
        var chars = new char[bytes.Length / sizeof(char)];
        Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
        return new string(chars);
    }

}
