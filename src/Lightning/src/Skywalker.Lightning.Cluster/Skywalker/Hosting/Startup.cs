using Microsoft.Extensions.DependencyInjection;
using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using Skywalker.Lightning.LoadBalance;

namespace Skywalker.Hosting
{
    class Startup
    {
        public void ConfigureService(IServiceCollection services)
        {

            services.AddSingleton<ILightningCluster, LightningCluster>();
            services.AddSingleton<IAddressSelector, PollingAddressSelector>();
            services.AddSingleton<ILightningClusterDescriptor, LightningClusterDescriptor>();
        }
    }
}
