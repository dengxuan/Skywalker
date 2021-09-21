using Microsoft.Extensions.Configuration;
using Skywalker.Spider;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Pipelines;
using Skywalker.Spider.Pipelines.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderIServiceCollectionExtensions
    {

        private static IServiceCollection AddSpiderServices(IServiceCollection services)
        {
            services.AddDuplicateRemover();
            services.AddScheduler();
            services.AddSingleton<IRequestHasher, RequestHasher>();
            services.AddSingleton<ISpiderBuilder>(new SpiderBuilder(services));
            services.AddHostedService<HostedSpiderService>();
            return services;
        }

        public static IServiceCollection AddSpider<TSpider>(this IServiceCollection services, Action<ISpiderBuilder> builder, Action<SpiderOptions> options)
        {
            services.Configure(options);
            AddSpiderServices(services);
            ISpiderBuilder spiderBuilder = services.GetSingletonInstance<ISpiderBuilder>();
            builder?.Invoke(spiderBuilder);
            return services;
        }

        public static IServiceCollection AddSpider(this IServiceCollection services, Action<ISpiderBuilder> builder)
        {
            IConfiguration? configuration = services.GetConfiguration();
            IConfigurationSection? section = configuration?.GetSection("SpiderOptions");
            services.Configure<SpiderOptions>(section);
            AddSpiderServices(services);
            ISpiderBuilder spiderBuilder = services.GetSingletonInstance<ISpiderBuilder>();
            builder?.Invoke(spiderBuilder);
            return services;
        }
    }
}
