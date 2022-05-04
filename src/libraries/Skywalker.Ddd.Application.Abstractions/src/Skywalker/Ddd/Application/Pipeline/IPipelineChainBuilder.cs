﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Application.Pipeline;

/// <summary>
/// Represents a builder to build an pipeline chain.
/// </summary>
public interface IPipelineChainBuilder: ISingletonDependency
{
    /// <summary>
    /// Gets the service provider to get dependency services.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Register specified interceptor.
    /// </summary>
    /// <param name="pipeline">The pipeline to register.</param>
    /// <returns>The interceptor chain builder with registered intercetor.</returns>
    IPipelineChainBuilder Use(PipelineDelegate pipeline);

    IPipelineChainBuilder Use<TInterceptor>();

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
