using Caching.Abstractions;
using Caching.Redis.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Caching.Redis;

public class RedisCachingProvider : CachingProvider
{
    private readonly IServiceProvider _iocResolver;

    public RedisCachingProvider(IServiceProvider iocResolver)
    {
        _iocResolver = iocResolver;
    }

    protected override ICaching CreateCacheImplementation(string name)
    {
        return new RedisCaching(name, _iocResolver.GetRequiredService<IRedisDatabaseProvider>(), _iocResolver.GetRequiredService<ICachingSerializer>());
    }
}
