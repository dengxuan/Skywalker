using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies;

public class ProxiedHttpClientHandler : HttpClientHandler
{
    private readonly IProxyPool _proxies;

    public ProxiedHttpClientHandler(IProxyPool proxies)
    {
        _proxies = proxies;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (Proxy is WebProxy webProxy && webProxy?.Address != null)
        {
            await _proxies.BackAsync(webProxy.Address, response.StatusCode);
        }

        return response;
    }
}
