using Skywalker.Spider.Pipelines.Abstractions;
using System;

namespace Skywalker.Spider.Pipelines.Extensions;

/// <summary>
/// Extension methods for adding terminal middleware.
/// </summary>
public static class RunExtensions
{
    /// <summary>
    /// Adds a terminal middleware delegate to the application's request pipeline.
    /// </summary>
    /// <param name="app">The <see cref="IPipelineChainBuilder"/> instance.</param>
    /// <param name="handler">A delegate that handles the request.</param>
    public static void Run(this IPipelineChainBuilder app, PipelineDelegate handler)
    {
        if (app == null)
        {
            throw new ArgumentNullException(nameof(app));
        }

        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        app.Use(_ => handler);
    }
}
