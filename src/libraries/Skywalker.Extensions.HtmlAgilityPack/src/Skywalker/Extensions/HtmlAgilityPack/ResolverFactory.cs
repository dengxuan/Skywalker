using Skywalker.Extensions.HtmlAgilityPack.Abstractions;
using System;
using System.Net.Http;

namespace Skywalker.Extensions.HtmlAgilityPack;

public class ResolverFactory : IResolverFactory
{

    public IResolver CreateResolver(HttpResponseMessage httpResponse)
    {
        throw new NotImplementedException();
    }

    public IResolver CreateResolver(string contentType)
    {
        throw new NotImplementedException();
    }
}
