using Skywalker.Lightning.Cluster;

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
        LightningAddress? GetAddressAsync(string serviceName);
    }
}
