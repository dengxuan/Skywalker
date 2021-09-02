using Skywalker.Spider.Proxies.Abstractions;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader.Proxies
{
    public class ProxiedHttpClientHandler : HttpClientHandler
    {
        private readonly IProxyPool _proxies;

        internal ProxiedHttpClientHandler(IProxyPool proxies)
        {
            _proxies = proxies;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var isProxyTestUrl = request.Headers.TryGetValues("PROXY_TEST_URI", out _);
            if (isProxyTestUrl)
            {
                request.Headers.Remove("PROXY_TEST_URI");
            }
            var response = await base.SendAsync(request, cancellationToken);
            
            if(Proxy is WebProxy webProxy && webProxy.Address != null)
            {
                await _proxies.BackAsync(webProxy.Address, response.StatusCode);
            }
            return response;
        }
    }
}
