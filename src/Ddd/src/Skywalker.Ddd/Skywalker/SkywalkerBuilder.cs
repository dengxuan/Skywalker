using Microsoft.Extensions.DependencyInjection;

namespace Skywalker
{
    public class SkywalkerBuilder
    {
        public IServiceCollection Services { get; }

        public SkywalkerBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
