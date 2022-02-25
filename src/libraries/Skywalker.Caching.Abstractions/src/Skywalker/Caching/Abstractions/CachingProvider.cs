using System.Collections.Concurrent;

namespace Skywalker.Caching.Abstractions;

public abstract class CachingProvider : ICachingProvider
{
    private readonly ConcurrentDictionary<string, ICaching> _cachings = new();

    public ICaching GetCaching(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        return _cachings.GetOrAdd(name, (cacheName) =>
        {
            var cache = CreateCacheImplementation(cacheName);
            return cache;
        });
    }

    /// <summary>
    /// Used to create actual cache implementation.
    /// </summary>
    /// <param name="name">Name of the cache</param>
    /// <returns>Cache object</returns>
    protected abstract ICaching CreateCacheImplementation(string name);
}
