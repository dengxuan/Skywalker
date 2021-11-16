using Caching.Redis.Abstractions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using System;

namespace Caching.Redis
{
    public class RedisDatabaseProvider : IRedisDatabaseProvider
    {
        private readonly RedisOptions _options;
        private readonly Lazy<ConnectionMultiplexer> _connectionMultiplexer;

        public RedisDatabaseProvider(IOptionsMonitor<RedisOptions> options)
        {
            _options = options.CurrentValue;
            options.OnChange(listener =>
            {
                _options.ConnectionString = listener.ConnectionString;
            });
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
