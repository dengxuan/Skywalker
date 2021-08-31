using Skywalker.EventBus.Abstractions;
using Skywalker.Spider;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderIServiceCollectionExtensions
    {
        private static IServiceCollection AddSpiderServices(IServiceCollection services)
        {
            services.AddSingleton<IEventHandler<Response>, ResponseEventHandler>();
            services.AddHostedService<SpiderBackgroundService>();
            return services;
        }

        public static IServiceCollection AddSpider(this IServiceCollection services, Action<SpiderOptions> options)
        {
            ISpiderBuilder spiderBuilder = services.AddSpiderCore(options);
            spiderBuilder.AddScheduler();
            AddSpiderServices(spiderBuilder.Services);
            return services;
        }

        public static IServiceCollection AddSpider(this IServiceCollection services)
        {
            ISpiderBuilder spiderBuilder = services.AddSpiderCore();
            spiderBuilder.AddScheduler();
            AddSpiderServices(spiderBuilder.Services);
            return services;
        }
    }
}
