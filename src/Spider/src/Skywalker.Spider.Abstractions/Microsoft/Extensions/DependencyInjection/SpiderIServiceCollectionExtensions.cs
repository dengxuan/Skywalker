using Microsoft.Extensions.Configuration;
using Skywalker.Spider;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderIServiceCollectionExtensions
    {
        private static ISpiderBuilder AddSpiderServices(IServiceCollection services)
        {
            services.AddSingleton<IRequestHasher, RequestHasher>();
            services.AddSingleton<InProgressRequests>();
            ISpiderBuilder spiderBuilder = new SpiderBuilder(services);
            return spiderBuilder;
        }

        public static ISpiderBuilder AddSpiderCore(this IServiceCollection services, Action<SpiderOptions> options)
        {
            services.Configure(options);
            return AddSpiderServices(services);
        }

        public static ISpiderBuilder AddSpiderCore(this IServiceCollection services)
        {
            IConfiguration? configuration = services.GetConfiguration();
            IConfigurationSection? section = configuration?.GetSection("SpiderOptions");
            services.Configure<SpiderOptions>(section);
            return AddSpiderServices(services);
        }
    }
}
