using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;

namespace Skywalker.Lightning.LoadBalance
{
    public abstract class AddressSelector : IAddressSelector
    {

        protected ILightningCluster LightningCluster { get; }

        protected AddressSelector(ILightningCluster lightningCluster)
        {
            LightningCluster = lightningCluster;
        }

        public abstract LightningAddress? GetAddressAsync(string sreviceName);
    }
}
