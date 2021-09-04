using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Extensions.HtmlAgilityPack;
using Skywalker.Extensions.HtmlAgilityPack.Abstractions;
using Skywalker.Spider.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Spider.Pipeline.DataResolver.Abstractions;

public abstract class HttpResolver : Pipeline
{
    private readonly List<Func<Request, bool>> _requiredValidator;

    /// <summary>
    /// 数据解析
    /// </summary>
    /// <param name="context">处理上下文</param>
    /// <returns></returns>
    protected abstract Task ResolveAsync(PipelineContext context, IResolver resolver);

    protected HttpResolver(ILogger<HttpResolver> logger) : base(logger)
    {
        _requiredValidator = new List<Func<Request, bool>>();
    }

    /// <summary>
    /// 数据解析
    /// </summary>
    /// <param name="context">处理上下文</param>
    /// <returns></returns>
    public override async Task HandleAsync(PipelineContext context)
    {
        context.NotNull(nameof(context));
        context.Response.NotNull(nameof(context.Response));

        if (!IsValidRequest(context.Request))
        {
            Logger.LogInformation(
                $"{GetType().Name} ignore parse request {context.Request.RequestUri}, {context.Request.Hash}");
            return;
        }
        IResolverFactory resolverFactory = context.ServiceProvider.GetRequiredService<IResolverFactory>();
        var contentType = context.Response.Headers[HeaderNames.ContentType];
        IResolver resolver = resolverFactory.CreateResolver(contentType);
        SelectorFactory.XPath("").Select(context.Response.ReadAsString());
        await ResolveAsync(context, resolver);
    }

    public bool IsValidRequest(Request request)
    {
        return _requiredValidator.Count <= 0 ||
               _requiredValidator.Any(validator => validator(request));
    }
}
