using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies.Abstractions
{
    public interface IProxyStorage
    {
        /// <summary>
        /// Create new proxy
        /// </summary>
        /// <param name="proxy">New avaliable proxy.</param>
        /// <returns>A task when created.</returns>
        Task CreateAsync(ProxyEntry proxy);

        /// <summary>
        /// Update expired proxy.
        /// </summary>
        /// <param name="proxy">Expired proxy.</param>
        /// <returns>A task when updated.</returns>
        Task UpdateAsync(ProxyEntry proxy);

        /// <summary>
        /// Find abailable proxies from storage.
        /// </summary>
        /// <returns>Available proxies or empty</returns>
        Task<IEnumerable<ProxyEntry>> GetAvailablesAsync();
    }
}
