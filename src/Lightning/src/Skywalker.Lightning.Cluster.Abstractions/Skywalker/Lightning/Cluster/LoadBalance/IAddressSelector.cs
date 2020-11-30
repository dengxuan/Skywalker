using Skywalker.Lightning.Cluster;
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
        /// <returns></returns>
        Task<LightningAddress> GetAddressAsync(string serviceName);
    }
}
