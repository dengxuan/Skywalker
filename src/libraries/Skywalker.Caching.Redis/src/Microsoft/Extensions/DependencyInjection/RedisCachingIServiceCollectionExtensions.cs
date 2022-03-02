using Skywalker.Caching;
using Skywalker.Caching.Abstractions;
using Skywalker.Caching.Redis;
using Skywalker.Caching.Redis.Abstractions;

namespace Microsoft.Extensions.DependencyInjection;

public static class RedisCachingIServiceCollectionExtensions
{
    public static IServiceCollection AddRedisCachingServices(this IServiceCollection services)
    {
        services.AddSingleton<ICachingProvider, RedisCachingProvider>();
        services.AddSingleton<IRedisDatabaseProvider, RedisDatabaseProvider>();
        services.AddSingleton<ICachingSerializer, JsonCachingSerializer>();
        return services;
    }

    public static IServiceCollection AddRedisCaching(this IServiceCollection services, Action<RedisOptions> configure)
    {
        services.Configure(configure);
        return services.AddRedisCachingServices();
    }

    public static IServiceCollection AddRedisCaching(this IServiceCollection services)
    {
        var configuration = services.GetConfiguration();
        services.Configure<RedisOptions>(configuration?.GetSection(nameof(RedisOptions)));
        return services.AddRedisCachingServices();
    }
}
