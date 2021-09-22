using Microsoft.Extensions.Options;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Skywalker.Spider.Proxies
{
    public class BeikeProxySupplier : IProxySupplier
    {
        private readonly ProxyOptions _options;
        private readonly IHttpClientFactory _httpClientFactory;

        public BeikeProxySupplier(IOptions<ProxyOptions> proxyOptions, IHttpClientFactory httpClientFactory)
        {
            _options = proxyOptions.Value;
            _httpClientFactory = httpClientFactory;

        }
        public async Task<IEnumerable<ProxyEntry>> GetProxiesAsync()
        {
            HttpClient client = _httpClientFactory.CreateClient();
            var apiAddress = _options.ApiAddresses?["Beike"];
            HttpRequestMessage httpRequest = new(HttpMethod.Get, apiAddress);
            HttpResponseMessage httpResponse = await client.SendAsync(httpRequest);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return Enumerable.Empty<ProxyEntry>();
            }

            Result? result = await httpResponse.Content.ReadFromJsonAsync<Result>();
            if (result?.Code == 1)
            {
                List<ProxyEntry> entries = new();
                foreach (var item in result.Data!)
                {
                    UriBuilder uriBuilder = new("http", item.Ip, item.Port);

                    entries.Add(new(uriBuilder.Uri, DateTime.Parse(item.ExpireTime)));
                }
                return entries;
            }
            return Enumerable.Empty<ProxyEntry>();
        }
    }
}
