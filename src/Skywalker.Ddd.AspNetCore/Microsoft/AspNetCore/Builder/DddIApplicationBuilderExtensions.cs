// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.AspNetCore.Uow;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class DddIApplicationBuilderExtensions
{
    /// <summary>
    /// Adds unit of work middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UnitOfWorkMiddleware>();
    }

    /// <summary>
    /// Adds all Skywalker middlewares to the pipeline: exception handling + unit of work.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSkywalker(this IApplicationBuilder app)
    {
        ExceptionHandlingApplicationBuilderExtensions.UseSkywalkerExceptionHandling(app);
        app.UseUnitOfWork();
        return app;
    }
}
