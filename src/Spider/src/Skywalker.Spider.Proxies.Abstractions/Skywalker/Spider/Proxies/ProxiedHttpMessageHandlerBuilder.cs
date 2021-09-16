using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Skywalker.Spider.Proxies.Abstractions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Skywalker.Spider.Proxies;

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

        if (!Name!.StartsWith("SPIDER_PROXY_"))
        {
            PrimaryHandler = new HttpClientHandler();
            return CreateHandlerPipeline(PrimaryHandler, AdditionalHandlers);
        }

        var uri = Name!.RemovePreFix("SPIDER_PROXY_");
        PrimaryHandler = new ProxiedHttpClientHandler(_proxies)
        {
            UseProxy = true,
            Proxy = new WebProxy(uri)
        };

        return CreateHandlerPipeline(PrimaryHandler, AdditionalHandlers);
    }
}
