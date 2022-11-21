using Microsoft.Extensions.Caching.Distributed;
using Skywalker.Settings.Abstractions;

namespace Skywalker.Settings.Redis;

internal class RedisSettingStore : ISettingStore
{
    private readonly IDistributedCache _distributedCache;

    public RedisSettingStore(IDistributedCache distributedCache) => _distributedCache = distributedCache;

    public async Task<List<SettingValue>> GetAllAsync(string[] names, string? providerName, string? providerKey)
    {
        var results = new List<SettingValue>();
        foreach (var name in names)
        {
            var value = await GetOrNullAsync(name, providerName, providerKey);
            if(value == null)
            {
                continue;
            }
            results.Add(new SettingValue(name, value));
        }
        return results;
    }

    public async Task<string?> GetOrNullAsync(string name, string? providerName, string? providerKey)
    {
        var key = $"{providerName}:{providerKey}:{name}";
        return await _distributedCache.GetStringAsync(key);
    }
}
