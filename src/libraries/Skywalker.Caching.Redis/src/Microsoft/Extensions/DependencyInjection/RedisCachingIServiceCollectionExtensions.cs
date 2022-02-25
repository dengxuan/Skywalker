using Skywalker.Caching;
using Skywalker.Caching.Abstractions;
using Skywalker.Caching.Redis;
using Skywalker.Caching.Redis.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisCachingIServiceCollectionExtensions
{
    public static void AddRedisCaching(this IServiceCollection services, Action<RedisOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<ICachingProvider, RedisCachingProvider>();
        services.AddSingleton<IRedisDatabaseProvider, RedisDatabaseProvider>();
        services.AddSingleton<ICachingSerializer, JsonCachingSerializer>();
    }
}
