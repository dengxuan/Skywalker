using Microsoft.Extensions.Logging;
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
        private readonly ILogger<BeikeProxySupplier> _logger;

        public BeikeProxySupplier(IOptions<ProxyOptions> proxyOptions, IHttpClientFactory httpClientFactory, ILogger<BeikeProxySupplier> logger)
        {
            _options = proxyOptions.Value;
            _httpClientFactory = httpClientFactory;
            _logger = logger;

        }
        public async Task<IEnumerable<ProxyEntry>> GetProxiesAsync()
        {
            HttpClient client = _httpClientFactory.CreateClient();
            var apiAddress = _options.ApiAddresses?["Beike"];
            _logger.LogInformation("Get Proxies: {apiAddress}", apiAddress);
            HttpResponseMessage httpResponse = await client.GetAsync(apiAddress);

            if (!httpResponse.IsSuccessStatusCode)
            {
                return Enumerable.Empty<ProxyEntry>();
            }
            var jsonContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Get Proxies: {apiAddress} {jsonContent}", apiAddress, jsonContent);
            Result? result = JsonSerializer.Deserialize<Result>(jsonContent);
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
