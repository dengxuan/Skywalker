using Microsoft.Extensions.Logging;
using Skywalker.Caching.Abstractions;

namespace Skywalker.Ddd.Application.Pipeline;

public class CachingPipelineBehavior
{
    private readonly ICachingProvider _cachingProvider;
    private readonly ILogger<CachingPipelineBehavior> _logger;

    private readonly InterceptDelegate _next;

    public CachingPipelineBehavior(InterceptDelegate next, ICachingProvider cachingProvider, ILogger<CachingPipelineBehavior> logger)
    {
        _next = next;
        _cachingProvider = cachingProvider;
        _logger = logger;
    }

    public async ValueTask InvokeAsync(PipelineContext context)
    {
        _logger.LogInformation("Begin CachingPipelineBehavior");
        await _next(context);
        return;
        //var caching = _cachingProvider.GetCaching("");
        //context.ReturnValue = caching.Get("");
        //await _next(context);
    }
}
