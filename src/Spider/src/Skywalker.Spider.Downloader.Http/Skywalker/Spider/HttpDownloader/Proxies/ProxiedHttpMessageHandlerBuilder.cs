using Microsoft.Extensions.Http;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Skywalker.Spider.HttpDownloader.Proxies;

internal class ProxiedHttpMessageHandlerBuilder : HttpMessageHandlerBuilder
{
    private readonly IProxyPool _proxies;

    public ProxiedHttpMessageHandlerBuilder(IProxyPool proxies)
    {
        _proxies = proxies.NotNull(nameof(proxies));
    }

    public override string? Name { get; set; }

    public override HttpMessageHandler? PrimaryHandler { get; set; }

    public override IList<DelegatingHandler> AdditionalHandlers => new List<DelegatingHandler>();

    public override HttpMessageHandler Build()
    {
        if (PrimaryHandler != null)
        {
            return CreateHandlerPipeline(PrimaryHandler, AdditionalHandlers);
        }
        var uri = Name!.Replace("SPIDER_PROXY_", string.Empty);
        PrimaryHandler = new ProxiedHttpClientHandler(_proxies)
        {
            UseProxy = true,
            Proxy = new WebProxy(uri)
        };

        var result = CreateHandlerPipeline(PrimaryHandler, AdditionalHandlers);
        return result;
    }
}
