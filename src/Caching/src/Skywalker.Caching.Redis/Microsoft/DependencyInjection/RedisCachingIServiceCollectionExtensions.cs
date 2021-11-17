using Caching;
using Caching.Abstractions;
using Caching.Redis;
using Caching.Redis.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Microsoft.DependencyInjection
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
