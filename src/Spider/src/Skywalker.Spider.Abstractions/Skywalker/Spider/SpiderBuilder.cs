using Microsoft.Extensions.DependencyInjection;
using Skywalker.Spider.Abstractions;

namespace Skywalker.Spider
{
    public class SpiderBuilder : ISpiderBuilder
    {
        public IServiceCollection Services { get; }

        internal SpiderBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
