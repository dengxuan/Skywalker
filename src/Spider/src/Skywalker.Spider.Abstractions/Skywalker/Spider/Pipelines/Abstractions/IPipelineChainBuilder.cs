using System;

namespace Skywalker.Spider.Pipelines.Abstractions;

/// <summary>
/// Represents a builder to build an interceptor chain.
/// </summary>
public interface IPipelineChainBuilder
{
    /// <summary>
    /// Gets the service provider to get dependency services.
    /// </summary>
    IServiceProvider ApplicationServices { get; }

    /// <summary>
    /// Register specified interceptor.
    /// </summary>
    /// <param name="middlewore">The interceptor to register.</param>
    /// <returns>The interceptor chain builder with registered intercetor.</returns>
    IPipelineChainBuilder Use(Func<PipelineDelegate, PipelineDelegate> middlewore);

    /// <summary>
    /// Build an interceptor chain using the registerd interceptors.
    /// </summary>
    /// <returns>A composite interceptor representing the interceptor chain.</returns>
    PipelineDelegate Build();

    /// <summary>
    /// Create a new interceptor chain builder which owns the same service provider as the current one.
    /// </summary>
    /// <returns>The new interceptor to create.</returns>
    IPipelineChainBuilder New();
}
