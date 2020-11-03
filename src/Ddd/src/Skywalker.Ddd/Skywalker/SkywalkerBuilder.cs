using Microsoft.Extensions.DependencyInjection;

namespace Skywalker
{
    public class SkywalkerBuilder
    {
        public IServiceCollection Services { get; }

        internal SkywalkerBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
