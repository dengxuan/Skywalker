using Skywalker.Caching.Abstractions;
using Skywalker.Caching.Redis.Abstractions;
using StackExchange.Redis;

namespace Skywalker.Caching.Redis;

internal class RedisCaching : ICaching
{
    private static readonly ReaderWriterLockSlim s_locker = new();

    private readonly IDatabase _database;

    private readonly ICachingSerializer _serializer;

    public string Name { get; }

    public TimeSpan? DefaultExpireTime { get; set; }

    internal RedisCaching(string name, IRedisDatabaseProvider redisCacheProvider, ICachingSerializer serializer)
    {
        _database = redisCacheProvider.GetDatabase();
        _serializer = serializer;
        Name = name;
        DefaultExpireTime = TimeSpan.FromHours(1);
    }

    public byte[] Get(string key)
    {
        try
        {
            s_locker.EnterReadLock();
            byte[] bytes = _database.StringGet($"{Name}@{key}");
            return bytes;
        }
        finally
        {
            s_locker.ExitReadLock();
        }
    }

    public async ValueTask<byte[]> GetAsync(string key)
    {
        try
        {
            s_locker.EnterReadLock();
            byte[] bytes = await _database.StringGetAsync($"{Name}@{key}");
            return bytes;
        }
        finally
        {
            s_locker.ExitReadLock();
        }
    }

    public TVaule? Get<TVaule>(string key)
    {
        var bytes = Get(key);
        if (bytes == null)
        {
            return default;
        }
        return _serializer.Deserialize<TVaule>(bytes);
    }

    public async ValueTask<TVaule?> GetAsync<TVaule>(string key)
    {
        var bytes = await GetAsync(key);
        if (bytes == null)
        {
            return default;
        }
        return _serializer.Deserialize<TVaule>(bytes);
    }

    public IEnumerable<TValue?> GetMany<TValue>(IEnumerable<string> keys)
    {
        var results = new List<TValue?>();
        foreach (var key in keys)
        {
            var redisValue = _database.StringGet(key);
            var value = _serializer.Deserialize<TValue>(redisValue);
            results.Add(value);
        }
        return results;
    }

    public async ValueTask<IEnumerable<TValue?>> GetManyAsync<TValue>(IEnumerable<string> keys)
    {
        var results = new List<TValue?>();
        foreach (var key in keys)
        {
            var redisValue = await _database.StringGetAsync(key);
            var value = _serializer.Deserialize<TValue>(redisValue);
            results.Add(value);
        }
        return results;
    }

    public void Set(string key, byte[] value, TimeSpan? expireTime = null)
    {
        if (value == null)
        {
            throw new Exception("Can not insert null values to the cache!");
        }
        _database.StringSet($"{Name}@{key}", value, expireTime ?? DefaultExpireTime);
    }

    public async Task SetAsync(string key, byte[] value, TimeSpan? expireTime = null)
    {
        if (value == null)
        {
            throw new Exception("Can not insert null values to the cache!");
        }
        await _database.StringSetAsync($"{Name}@{key}", value, expireTime ?? DefaultExpireTime);
    }

    public void Set<TVaule>(string key, TVaule value, TimeSpan? expireTime = null)
    {
        var bytes = _serializer.Serialize(value);
        Set(key, bytes, expireTime);
    }

    public async Task SetAsync<TVaule>(string key, TVaule value, TimeSpan? expireTime = null)
    {
        var bytes = _serializer.Serialize(value);
        await SetAsync(key, bytes, expireTime);
    }

    public void SetMany<TVaule>(IEnumerable<KeyValuePair<string, TVaule>> items, TimeSpan? expireTime = null)
    {
        foreach (var item in items)
        {
            var bytes = _serializer.Serialize(item.Value);
            Set(item.Key, bytes, expireTime);
        }
    }

    public Task SetManyAsync<TVaule>(IEnumerable<KeyValuePair<string, TVaule>> items, TimeSpan? expireTime = null)
    {
        var tasks = new List<Task>();
        foreach (var item in items)
        {
            var bytes = _serializer.Serialize(item.Value);
            tasks.Add(SetAsync(item.Key, bytes, expireTime));
        }
        return Task.WhenAll(tasks);
    }

    public TValue GetOrSet<TValue>(string key, Func<TValue> factory) where TValue : notnull
    {
        var value = Get<TValue>(key);
        if (value == null)
        {
            try
            {
                s_locker.EnterWriteLock();
                value = factory();
                Set(key, value);
            }
            finally
            {
                s_locker.ExitWriteLock();
            }
        }
        return value;
    }

    public async ValueTask<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory) where TValue : notnull
    {
        var value = await GetAsync<TValue>(key);
        if (value == null)
        {
            try
            {
                s_locker.EnterWriteLock();
                value = await factory();
                await SetAsync(key, value);
            }
            finally
            {
                s_locker.ExitWriteLock();
            }
        }
        return value;
    }

    public void Remove(string key)
    {
        _database.KeyDelete($"{Name}@{key}");
    }

    public async Task RemoveAsync(string key)
    {
        await _database.KeyDeleteAsync($"{Name}@{key}");
    }

    public void Clear()
    {
        _database.KeyDeleteWithPrefix($"{Name}*");
    }

    public Task ClearAsync()
    {
        return Task.Run(() =>
        {
            _database.KeyDeleteWithPrefix($"{Name}*");
        });
    }

    public void Dispose()
    {
        s_locker.Dispose();
    }
}
