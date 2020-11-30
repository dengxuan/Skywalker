using Skywalker.Lightning.Cluster;
using System.Threading.Tasks;

namespace Skywalker.Lightning.LoadBalance
{
    public abstract class AddressSelector : IAddressSelector
    {
        public abstract Task<LightningAddress> GetAddressAsync(string sreviceName);
    }
}
