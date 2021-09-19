using Skywalker.Spider.Pipelines.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skywalker.Spider.Pipelines;

/// <summary>
/// The default implementation of interceptor chain builder.
/// </summary>
public class PipelineChainBuilder : IPipelineChainBuilder
{
    private readonly IList<Func<PipelineDelegate, PipelineDelegate>> _components = new List<Func<PipelineDelegate, PipelineDelegate>>();

    /// <summary>
    /// Gets the service provider to get dependency services.
    /// </summary>
    public IServiceProvider ApplicationServices { get; }

    /// <summary>
    /// Create a new <see cref="PipelineChainBuilder"/>.
    /// </summary>
    /// <param name="serviceProvider">The service provider to get dependency services.</param>
    /// <exception cref="ArgumentNullException">The argument <paramref name="serviceProvider"/> is null.</exception>
    public PipelineChainBuilder(IServiceProvider serviceProvider)
    {
        ApplicationServices = serviceProvider.NotNull(nameof(serviceProvider));
    }


    /// <summary>
    /// Register specified interceptor.
    /// </summary>
    /// <param name="middlewore">The interceptor to register.</param>
    /// <returns>The interceptor chain builder with registered intercetor.</returns>
    /// <exception cref="ArgumentNullException">The argument <paramref name="middlewore"/> is null.</exception>
    public IPipelineChainBuilder Use(Func<PipelineDelegate, PipelineDelegate> middlewore)
    {
        _components.Add(middlewore.NotNull(nameof(middlewore)));
        return this;
    }

    /// <summary>
    /// Build an interceptor chain using the registerd interceptors.
    /// </summary>
    /// <returns>A composite interceptor representing the interceptor chain.</returns>
    public PipelineDelegate Build()
    {
        PipelineDelegate app = context => Task.CompletedTask;
        foreach (var it in _components.Reverse())
        {
            app = it(app);
        }
        return app;
    }

    /// <summary>
    /// Create a new interceptor chain builder which owns the same service provider as the current one.
    /// </summary>
    /// <returns>The new interceptor to create.</returns>
    public IPipelineChainBuilder New()
    {
        return new PipelineChainBuilder(ApplicationServices);
    }
}
