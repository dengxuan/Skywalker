using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using Skywalker.Lightning.Cluster.Internal;
using Skywalker.Lightning.LoadBalance;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class LightningClusterIServiceCollectionExtensions
    {
        public static IServiceCollection AddLightningCluster(this IServiceCollection services)
        {
            LightningDescriptorContainer descriptorContainer = new LightningDescriptorContainer();
            services.AddSingleton<ILightningDescriptorResolver>(descriptorContainer);
            services.AddSingleton<ILightningDescriptorContainer>(descriptorContainer);

            services.AddSingleton<ILightningCluster, LightningCluster>();
            services.AddSingleton<IAddressSelector, PollingAddressSelector>();
            return services;
        }
    }
}
