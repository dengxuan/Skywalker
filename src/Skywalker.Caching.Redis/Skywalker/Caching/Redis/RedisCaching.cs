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

    public byte[]? Get(string key)
    {
        return s_locker.ReadLocking(() =>
        {
            var bytes = _database.StringGet($"{Name}@{key}");
            return bytes;
        });
    }

    public async ValueTask<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return await s_locker.ReadLockingAsync(async () =>
        {
            var bytes = await _database.StringGetAsync($"{Name}@{key}");
            return bytes;
        });
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

    public async ValueTask<TVaule?> GetAsync<TVaule>(string key, CancellationToken cancellationToken = default)
    {
        var bytes = await GetAsync(key, cancellationToken);
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

    public async ValueTask<IEnumerable<TValue?>> GetManyAsync<TValue>(IEnumerable<string> keys, CancellationToken cancellationToken = default)
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
        s_locker.WriteLocking(() =>
        {
            if (value == null)
            {
                throw new Exception("Can not insert null values to the cache!");
            }
            _database.StringSet($"{Name}@{key}", value, expireTime ?? DefaultExpireTime);
        });
    }

    public Task SetAsync(string key, byte[] value, TimeSpan? expireTime = null, CancellationToken cancellationToken = default)
    {
        return s_locker.WriteLockingAsync(async () =>
        {
            if (value == null)
            {
                throw new Exception("Can not insert null values to the cache!");
            }
            await _database.StringSetAsync($"{Name}@{key}", value, expireTime ?? DefaultExpireTime);
        });
    }

    public void Set<TVaule>(string key, TVaule value, TimeSpan? expireTime = null)
    {
        var bytes = _serializer.Serialize(value);
        Set(key, bytes, expireTime);
    }

    public async Task SetAsync<TVaule>(string key, TVaule value, TimeSpan? expireTime = null, CancellationToken cancellationToken = default)
    {
        var bytes = _serializer.Serialize(value);
        await SetAsync(key, bytes, expireTime, cancellationToken);
    }

    public void SetMany<TVaule>(IEnumerable<KeyValuePair<string, TVaule>> items, TimeSpan? expireTime = null)
    {
        foreach (var item in items)
        {
            var bytes = _serializer.Serialize(item.Value);
            Set(item.Key, bytes, expireTime);
        }
    }

    public Task SetManyAsync<TVaule>(IEnumerable<KeyValuePair<string, TVaule>> items, TimeSpan? expireTime = null, CancellationToken cancellationToken = default)
    {
        var tasks = new List<Task>();
        foreach (var item in items)
        {
            var bytes = _serializer.Serialize(item.Value);
            tasks.Add(SetAsync(item.Key, bytes, expireTime, cancellationToken));
        }
        return Task.WhenAll(tasks);
    }

    public TValue GetOrSet<TValue>(string key, Func<TValue> factory)
    {
        var value = Get<TValue>(key);
        if (value == null)
        {
            try
            {
                s_locker.EnterWriteLock();
                value = factory();
                if(value != null)
                {
                    Set(key, value);
                }
            }
            finally
            {
                s_locker.ExitWriteLock();
            }
        }
        return value;
    }

    public async ValueTask<TValue> GetOrSetAsync<TValue>(string key, Func<Task<TValue>> factory, CancellationToken cancellationToken = default)
    {
        var value = await GetAsync<TValue>(key, cancellationToken);
        if (value != null)
        {
            return value;
        }
        return await s_locker.WriteLocking(async () =>
        {
            var data = await factory();
            if(data != null)
            {
                await SetAsync(key, data, cancellationToken: cancellationToken);
            }
            return data;
        });
    }

    public void Remove(string key)
    {
        _database.KeyDelete($"{Name}@{key}");
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await _database.KeyDeleteAsync($"{Name}@{key}");
    }

    public async Task RemoveManyAsync(string[] keys, CancellationToken cancellationToken = default)
    {
        foreach (var key in keys)
        {
            await RemoveAsync(key, cancellationToken);
        }
    }

    public void Clear()
    {
        _database.KeyDeleteWithPrefix($"{Name}*");
    }

    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            _database.KeyDeleteWithPrefix($"{Name}*");
        }, cancellationToken);
    }

    public void Dispose()
    {
        s_locker.Dispose();
    }
}
