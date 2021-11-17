using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd
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
