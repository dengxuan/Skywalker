using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Lightning
{
    public interface ILightningBuilder
    {
        public IServiceCollection Services { get; }
    }

    class LigntingBuilder : ILightningBuilder
    {
        internal LigntingBuilder(IServiceCollection services) => Services = services;

        public IServiceCollection Services { get; }
    }
}
