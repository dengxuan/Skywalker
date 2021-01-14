using System;

namespace Skywalker.Caching.Abstractions
{
    /// <summary>
    /// Defines a cache that can be store and get items by keys.
    /// </summary>
    public interface ICaching : IDisposable
    {
        string Name { get; }

        /// <summary>
        /// Default expire time of cache items.
        /// Default value:  60 minutes (1 hour).
        /// </summary>
        TimeSpan? DefaultExpireTime { get; set; }

        /// <summary>
        /// Gets an item from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item</returns>
        byte[] Get(string key);

        /// <summary>
        /// Saves/Overrides an item in the cache by a key.
        /// Use the expire times at <paramref name="expireTime"/>).
        /// If none of them is specified, then
        /// <see cref="DefaultExpireTime"/> will be used if it's not null.
        /// will be used.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="expireTime">Absolute expire time</param>
        void Set(string key, byte[] value, TimeSpan? expireTime = null);

        /// <summary>
        /// Removes a cache item by it's key.
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(string key);

        /// <summary>
        /// Clears all items in this cache.
        /// </summary>
        void Clear();
    }
}
