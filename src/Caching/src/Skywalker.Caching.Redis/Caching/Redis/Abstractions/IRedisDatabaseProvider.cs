using StackExchange.Redis;

namespace Skywalker.Caching.Redis.Abstractions
{
    public interface IRedisDatabaseProvider
    {
        /// <summary>
        /// Gets the database connection.
        /// </summary>
        IDatabase GetDatabase();
    }
}
