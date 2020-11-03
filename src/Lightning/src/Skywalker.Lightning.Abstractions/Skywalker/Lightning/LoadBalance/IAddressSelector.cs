using Skywalker.Lightning.Cluster;
using Skywalker.Lightning.Cluster.Abstractions;
using System.Threading.Tasks;

namespace Skywalker.Lightning.LoadBalance
{
    /// <summary>
    ///     server selector
    /// </summary>
    public interface IAddressSelector
    {
        /// <summary>
        ///     get server for specify route
        /// </summary>
        /// <param name="descriptor"></param>
        /// <returns></returns>
        Task<ILightningCluster> GetAddressAsync(ILightningClusterDescriptor descriptor, string serviceName);
    }
}
