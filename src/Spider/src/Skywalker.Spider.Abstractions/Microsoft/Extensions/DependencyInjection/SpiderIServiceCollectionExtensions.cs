using Microsoft.Extensions.Configuration;
using Skywalker.EventBus.Abstractions;
using Skywalker.Spider;
using Skywalker.Spider.Http;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderIServiceCollectionExtensions
    {

        private static IServiceCollection AddSpiderServices(IServiceCollection services)
        {
            services.AddSingleton<IRequestHasher, RequestHasher>();
            services.AddSingleton<InProgressRequests>();
            services.AddSingleton<IEventHandler<Response>, ResponseEventHandler>();
            services.AddHostedService<Spider>();
            return services;
        }

        public static IServiceCollection AddSpider<TSpider>(this IServiceCollection services, Action<SpiderOptions> options)
        {
            services.Configure(options);
            AddSpiderServices(services);
            return services;
        }

        public static IServiceCollection AddSpider(this IServiceCollection services)
        {
            IConfiguration? configuration = services.GetConfiguration();
            IConfigurationSection? section = configuration?.GetSection("SpiderOptions");
            services.Configure<SpiderOptions>(section);
            AddSpiderServices(services);
            return services;
        }
    }
}
