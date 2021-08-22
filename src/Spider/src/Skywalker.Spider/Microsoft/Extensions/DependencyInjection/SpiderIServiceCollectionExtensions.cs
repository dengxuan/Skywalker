using Skywalker.Spider;
using Skywalker.Spider.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderIServiceCollectionExtensions
    {
        public static IServiceCollection AddSpider(this IServiceCollection services, Action<SpiderOptions> options)
        {
            ISpiderBuilder spiderBuilder = services.AddSpiderCore(options);
            spiderBuilder.AddMessaging();
            spiderBuilder.AddScheduler();
            return services;
        }
    }
}
