using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies.Abstractions;

public interface IProxySupplier
{
    Task<IEnumerable<ProxyEntry>> GetProxiesAsync();
}
