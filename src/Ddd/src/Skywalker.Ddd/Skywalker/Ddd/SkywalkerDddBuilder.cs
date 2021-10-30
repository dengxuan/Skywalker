using Microsoft.Extensions.DependencyInjection;

namespace Skywalker
{
    public class SkywalkerDddBuilder
    {
        public IServiceCollection Services { get; }

        public SkywalkerDddBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}
