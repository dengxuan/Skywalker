using Skywalker.Caching.Abstractions;
using Skywalker.Caching.Redis.Abstractions;
using StackExchange.Redis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;

namespace Skywalker.Caching.Redis
{
    internal class RedisCaching : ICaching
    {
        private static ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();


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
                _locker.EnterReadLock();
                byte[] bytes = _database.StringGet($"{Name}@{key}");
                return bytes;
            }
            finally
            {
                _locker.ExitReadLock();
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
            _locker.Dispose();
        }

        public TVaule? Get<TVaule>(string key)
        {
            byte[] bytes = Get(key);
            if(bytes == null)
            {
                return default;
            }
            return _serializer.Deserialize<TVaule>(bytes);
        }

        public void Set<TVaule>(string key, TVaule value, TimeSpan? expireTime = null)
        {
            byte[] bytes = _serializer.Serialize(value);
            Set(key, bytes, expireTime);
        }

        public TValue GetOrSet<TValue>(string key, [NotNull] Func<TValue> factory) where TValue : notnull
        {
            TValue value = Get<TValue>(key);
            if (value == null)
            {
                try
                {
                    _locker.EnterWriteLock();
                    value = factory();
                    Set(key, value);
                }
                finally
                {
                    _locker.ExitWriteLock();
                }
            }
            return value;
        }
    }
}
