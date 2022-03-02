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

    public void Clear()
    {
        _database.KeyDeleteWithPrefix($"{Name}*");
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

    public void Remove(string key)
    {
        _database.KeyDelete($"{Name}@{key}");
    }

    public void Set(string key, byte[] value, TimeSpan? expireTime = null)
    {
        if (value == null)
        {
            throw new Exception("Can not insert null values to the cache!");
        }
        _database.StringSet($"{Name}@{key}", value, expireTime ?? DefaultExpireTime);
    }

    public void Dispose()
    {
        s_locker.Dispose();
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

    public void Set<TVaule>(string key, TVaule value, TimeSpan? expireTime = null)
    {
        var bytes = _serializer.Serialize(value);
        Set(key, bytes, expireTime);
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
}
