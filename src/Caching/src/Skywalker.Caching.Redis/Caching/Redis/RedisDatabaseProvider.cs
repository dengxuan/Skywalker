using Skywalker.Caching.Redis.Abstractions;
using StackExchange.Redis;
using System;

namespace Skywalker.Caching.Redis
{
    public class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        private readonly RedisOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public RedisDatabaseProvider(RedisOptions options)
        {
            _options = options;
            _connectionMultiplexer = new Lazy<ConnectionMultiplexer>(CreateConnectionMultiplexer);
        }
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        public IDatabase GetDatabase()
        {
            return _connectionMultiplexer.Value.GetDatabase();
        }

        private ConnectionMultiplexer CreateConnectionMultiplexer()
        {
            return ConnectionMultiplexer.Connect(_options.ConnectionString);
        }
    }
}
