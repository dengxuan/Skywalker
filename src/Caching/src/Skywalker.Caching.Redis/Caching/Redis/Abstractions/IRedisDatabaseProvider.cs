using StackExchange.Redis;

namespace Skywalker.Extensions.Caching.Redis.Abstractions
{
    public interface IRedisDatabaseProvider
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        IDatabase GetDatabase();
    }
}
