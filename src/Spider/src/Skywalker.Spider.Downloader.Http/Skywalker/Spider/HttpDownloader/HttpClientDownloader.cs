using Microsoft.Extensions.Logging;
using Skywalker.Spider.Downloader;
using Skywalker.Spider.Downloader.Abstractions;
using Skywalker.Spider.Http;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Skywalker.Spider.HttpDownloader;

public class HttpClientDownloader : IDownloader
{

    private readonly IProxyPool _proxies;

    private readonly IHttpClientFactory _httpClientFactory;

    private readonly ILogger _logger;

    public HttpClientDownloader(IProxyPool proxies, IHttpClientFactory httpClientFactory, ILogger<HttpClientDownloader> logger)
    {
        _proxies = proxies;
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    protected virtual async Task<HttpClient> CreateClientAsync(int timeout)
    {
        var proxy = await _proxies.GetAsync(timeout);
        if (proxy == null)
        {
            throw new Exception("获取代理失败");
        }
        string name = $"SPIDER_PROXY_{proxy}";

        return _httpClientFactory.CreateClient(name);
    }

    public async Task<Response> DownloadAsync(Request request)
    {
        try
        {
            using var httpClient = await CreateClientAsync(request.Timeout);
            using var httpRequest = request.ToHttpRequestMessage();
            var stopwatch = Stopwatch.StartNew();
            using var httpResponse = await httpClient.SendAsync(httpRequest);
            stopwatch.Stop();
            _logger.LogInformation("Request[{0}] {1} {2}ms download completed!", httpRequest.RequestUri, httpResponse.StatusCode, stopwatch.ElapsedMilliseconds);
            var response = await httpResponse.ToResponseAsync();
            response.ElapsedMilliseconds = (int)stopwatch.ElapsedMilliseconds;
            response.RequestHash = request.Hash;
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{request.RequestUri} download failed", request.RequestUri);
            return new Response
            {
                RequestHash = request.Hash,
                StatusCode = HttpStatusCode.Gone,
                ReasonPhrase = ex.Message,
                Version = HttpVersion.Version11
            };
        }
    }
}
