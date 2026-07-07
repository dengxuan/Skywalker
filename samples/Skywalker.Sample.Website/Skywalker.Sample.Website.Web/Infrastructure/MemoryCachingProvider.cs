using System.Collections.Concurrent;
using Skywalker.Caching.Abstractions;

namespace Skywalker.Sample.Website.Web.Infrastructure;

/// <summary>
/// 进程内内存缓存 provider：演示 Caching 家族的扩展点
/// （生产环境换 Skywalker.Caching.Redis 的 AddRedisCaching 即可，消费方代码不变）。
/// </summary>
public sealed class MemoryCachingProvider : CachingProvider
{
    protected override ICaching CreateCacheImplementation(string name) => new MemoryCaching(name);
}

internal sealed class MemoryCaching(string name) : ICaching
{
    private sealed record Entry(object? Value, DateTimeOffset? ExpiresAt)
    {
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTimeOffset.UtcNow;
    }

    private readonly ConcurrentDictionary<string, Entry> _store = new();

    public string Name { get; } = name;

    public TimeSpan? DefaultExpireTime { get; set; } = TimeSpan.FromHours(1);

    private Entry? GetEntry(string key)
    {
        if (_store.TryGetValue(key, out var entry))
        {
            if (!entry.IsExpired)
            {
                return entry;
            }

            _store.TryRemove(key, out _);
        }

        return null;
    }

    private void SetEntry(string key, object? value, TimeSpan? expireTime)
    {
        var expiry = expireTime ?? DefaultExpireTime;
        _store[key] = new Entry(value, expiry.HasValue ? DateTimeOffset.UtcNow.Add(expiry.Value) : null);
    }

    public byte[]? Get(string key) => GetEntry(key)?.Value as byte[];

    public ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default) => ValueTask.FromResult(Get(key));

    public TValue? Get<TValue>(string key) => GetEntry(key)?.Value is TValue value ? value : default;

    public ValueTask<TValue?> GetAsync<TValue>(string key, CancellationToken cancellationToken = default) => ValueTask.FromResult(Get<TValue>(key));

    public IEnumerable<TValue?> GetMany<TValue>(IEnumerable<string> keys) => keys.Select(Get<TValue>);

    public ValueTask<IEnumerable<TValue?>> GetManyAsync<TValue>(IEnumerable<string> keys, CancellationToken cancellationToken = default) =>
        ValueTask.FromResult(GetMany<TValue>(keys));

    public void Set(string key, byte[] value, TimeSpan? expireTime = null) => SetEntry(key, value, expireTime);

    public Task SetAsync(string key, byte[] value, TimeSpan? expireTime = null, CancellationToken cancellationToken = default)
    {
        Set(key, value, expireTime);
        return Task.CompletedTask;
    }

    public void Set<TValue>(string key, TValue value, TimeSpan? expireTime = null) => SetEntry(key, value, expireTime);

    public Task SetAsync<TValue>(string key, TValue value, TimeSpan? expireTime = null, CancellationToken cancellationToken = default)
    {
        Set(key, value, expireTime);
        return Task.CompletedTask;
    }

    public void SetMany<TValue>(IEnumerable<KeyValuePair<string, TValue>> items, TimeSpan? expireTime = null)
    {
        foreach (var item in items)
        {
            Set(item.Key, item.Value, expireTime);
        }
    }

    public Task SetManyAsync<TValue>(IEnumerable<KeyValuePair<string, TValue>> items, TimeSpan? expireTime = null, CancellationToken cancellationToken = default)
    {
        SetMany(items, expireTime);
        return Task.CompletedTask;
    }

    public TValue GetOrSet<TValue>(string key, Func<TValue> factory)
    {
        if (GetEntry(key)?.Value is TValue cached)
        {
            return cached;
        }

        var value = factory();
        Set(key, value);
        return value;
    }

    public async ValueTask<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory, CancellationToken cancellationToken = default)
    {
        if (GetEntry(key)?.Value is TValue cached)
        {
            return cached;
        }

        var value = await factory();
        Set(key, value);
        return value;
    }

    public void Remove(string key) => _store.TryRemove(key, out _);

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        Remove(key);
        return Task.CompletedTask;
    }

    public Task RemoveManyAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        foreach (var key in keys)
        {
            Remove(key);
        }

        return Task.CompletedTask;
    }

    public void Clear() => _store.Clear();

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        Clear();
        return Task.CompletedTask;
    }

    public void Dispose() => _store.Clear();
}
