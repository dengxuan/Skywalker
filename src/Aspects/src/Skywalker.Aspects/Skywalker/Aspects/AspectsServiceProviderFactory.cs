using Microsoft.Extensions.DependencyInjection;
using System;

namespace Skywalker.Aspects
{
    public class AspectsServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.BuildAspectServiceProvider();
        }
    }
}
