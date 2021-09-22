using Skywalker.Spider.Proxies;
using Skywalker.Spider.Proxies.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderProxyIServiceCollectionExtensions
    {
        public static IServiceCollection AddJiguangProxy(this IServiceCollection services, Action<ProxyOptions> options)
        {
            services.AddProxyCore(options);
            services.AddSingleton<IProxySupplier, JiguangProxySupplier>();
            return services;
        }

        public static IServiceCollection AddJiguangProxy(this IServiceCollection services)
        {
            services.AddProxyCore();
            services.AddHttpClient();
            services.AddSingleton<IProxySupplier, JiguangProxySupplier>();
            return services;
        }
    }
}
