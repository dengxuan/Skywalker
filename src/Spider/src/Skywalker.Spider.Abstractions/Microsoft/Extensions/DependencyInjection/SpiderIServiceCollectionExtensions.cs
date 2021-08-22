using Skywalker.Spider;
using Skywalker.Spider.Abstractions;
using Skywalker.Spider.Http;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SpiderIServiceCollectionExtensions
    {
        public static ISpiderBuilder AddSpiderCore(this IServiceCollection services, Action<SpiderOptions> options)
        {
            services.Configure(options);
            services.AddSingleton<IRequestHasher, RequestHasher>();
            ISpiderBuilder spiderBuilder = new SpiderBuilder(services);
            return spiderBuilder;
        }
    }
}
