using Microsoft.Extensions.DependencyInjection;
using Skywalker.Extensions.Caching.Abstractions;
using Skywalker.Extensions.Caching.Redis.Abstractions;
using System;

namespace Skywalker.Extensions.Caching.Redis
{
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
}
