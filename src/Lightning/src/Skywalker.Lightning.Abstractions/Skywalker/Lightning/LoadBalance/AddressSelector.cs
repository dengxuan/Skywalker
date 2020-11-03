using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using System.Threading.Tasks;

namespace Skywalker.Lightning.LoadBalance
{
    public abstract class AddressSelector : IAddressSelector
    {
        public abstract Task<ILightningCluster> GetAddressAsync(ILightningClusterDescriptor descriptor, string sreviceName);
    }
}
