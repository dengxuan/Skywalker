using Skywalker.Spider.Pipelines.Abstractions;
using System;
using System.Threading.Tasks;

namespace Skywalker.Spider.Pipelines.Extensions;

/// <summary>
/// Extension methods for adding middleware.
/// </summary>
public static class UseExtensions
{
    /// <summary>
    /// Adds a middleware delegate defined in-line to the application's request pipeline.
    /// If you aren't calling the next function, use <see cref="RunExtensions.Run(IPipelineChainBuilder, PipelineDelegate)"/> instead.
    /// <para>
    /// Prefer using <see cref="Use(IPipelineChainBuilder, Func{PipelineContext, PipelineDelegate, Task})"/> for better performance as shown below:
    /// <code>
    /// app.Use((context, next) =>
    /// {
    ///     return next(context);
    /// });
    /// </code>
    /// </para>
    /// </summary>
    /// <param name="app">The <see cref="IPipelineChainBuilder"/> instance.</param>
    /// <param name="middleware">A function that handles the request and calls the given next function.</param>
    /// <returns>The <see cref="IPipelineChainBuilder"/> instance.</returns>
    public static IPipelineChainBuilder Use(this IPipelineChainBuilder app, Func<PipelineContext, Func<Task>, Task> middleware)
    {
        return app.Use(next =>
        {
            return context =>
            {
                return middleware(context, () => next(context));
            };
        });
    }

    /// <summary>
    /// Adds a middleware delegate defined in-line to the application's request pipeline.
    /// If you aren't calling the next function, use <see cref="RunExtensions.Run(IPipelineChainBuilder, PipelineDelegate)"/> instead.
    /// </summary>
    /// <param name="app">The <see cref="IPipelineChainBuilder"/> instance.</param>
    /// <param name="middleware">A function that handles the request and calls the given next function.</param>
    /// <returns>The <see cref="IPipelineChainBuilder"/> instance.</returns>
    public static IPipelineChainBuilder Use(this IPipelineChainBuilder app, Func<PipelineContext, PipelineDelegate, Task> middleware)
    {
        return app.Use(next => context => middleware(context, next));
    }
}
