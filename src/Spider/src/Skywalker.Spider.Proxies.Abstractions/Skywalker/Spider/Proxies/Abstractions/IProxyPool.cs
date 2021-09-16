using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies.Abstractions;

public interface IProxyPool
{
    Task InitializeAsync();

    Task<Uri?> GetAsync(int seconds);

    Task<int> SetAsync(IEnumerable<ProxyEntry> proxies);

    Uri? Get();

    /// <summary>
    /// 返还代理到代理池中
    /// </summary>
    /// <param name="proxy">代理</param>
    /// <param name="statusCode">状态码</param>
    /// <returns></returns>
    Task BackAsync(Uri proxy, HttpStatusCode statusCode);

    /// <summary>
    /// 从代理池中回收过期代理
    /// </summary>
    /// <param name="proxy">待回收代理</param>
    /// <returns></returns>
    Task RecycleAsync(Uri proxy);
}
