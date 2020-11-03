using Skywalker.Lightning;
using Skywalker.Lightning.Abstractions;
using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using Skywalker.Lightning.LoadBalance;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static ILightningBuilder AddLighting(this IServiceCollection services, Action<ILightningBuilder> lightner)
        {
            services.AddSingleton<ILightningServiceNameGenerator, LightningServiceNameGenerator>();
            services.AddSingleton<ILightningCluster, LightningCluster>();
            services.AddSingleton<IAddressSelector, PollingAddressSelector>();
            services.AddSingleton<ILightningClusterDescriptor, LightningClusterDescriptor>();
            LigntingBuilder lignting = new LigntingBuilder(services);
            lightner?.Invoke(lignting);
            return lignting;
        }
    }
}
