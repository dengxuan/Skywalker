using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Skywalker.Spider.Proxies;
using Skywalker.Spider.Proxies.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderProxyIServiceCollectionExtensions
    {
        private static IServiceCollection AddProxyServices(this IServiceCollection services)
        {
            services.AddSingleton<IProxyPool, ProxyPool>();
            services.AddSingleton<IProxyStorage, MysqlProxyStorage>();
            services.AddSingleton<IProxyValidator, DefaultProxyValidator>();
            services.AddTransient<HttpMessageHandlerBuilder, ProxiedHttpMessageHandlerBuilder>();
            services.AddHostedService<ProxyBackgroundService>();
            return services;
        }

        public static IServiceCollection AddProxyCore(this IServiceCollection services, Action<ProxyOptions> options)
        {
            services.Configure(options);
            return services.AddProxyServices();
        }

        public static IServiceCollection AddProxyCore(this IServiceCollection services)
        {
            IConfiguration? configuration = services.GetConfiguration();
            IConfigurationSection? section = configuration?.GetSection("ProxyOptions");
            services.Configure<ProxyOptions>(section);
            return services.AddProxyServices();
        }
    }
}
