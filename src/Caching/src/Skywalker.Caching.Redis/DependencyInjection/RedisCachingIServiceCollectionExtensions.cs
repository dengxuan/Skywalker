using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.Caching;
using Skywalker.Extensions.Caching.Abstractions;
using Skywalker.Extensions.Caching.Redis;
using Skywalker.Extensions.Caching.Redis.Abstractions;
using System;

namespace Skywalker.Extensions.DependencyInjection
{
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
}
