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
        private readonly IHttpClientFactory _httpClientFactory;

        public BeikeProxySupplier(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

        }
        public async Task<IEnumerable<ProxyEntry>> GetProxiesAsync()
        {
            HttpClient client = _httpClientFactory.CreateClient();
            Dictionary<string, string> keyValuePairs = new()
            {
                { "server_id", "15143" },
                { "user_id", "20210804000102648756" },
                { "token", "w2mQEnktl5DjBr61" },
                { "num", "1" },
                { "format", "json" },
                { "protocol", "1" },
                { "jsonexpiretime", "1" }
            };
            string query = keyValuePairs.Select(selector => $"{selector.Key}={selector.Value}").JoinAsString("&");
            HttpRequestMessage httpRequest = new(HttpMethod.Get, $"http://getip.beikeruanjian.com/getip/?{query}");
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
