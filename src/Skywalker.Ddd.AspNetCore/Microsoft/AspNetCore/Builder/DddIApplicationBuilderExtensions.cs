// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.AspNetCore.ExceptionHandling;
using Skywalker.Ddd.AspNetCore.Uow;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for <see cref="IApplicationBuilder"/>.
/// </summary>
public static class DddIApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Skywalker exception handling middleware to the pipeline.
    /// This middleware handles both exceptions and HTTP error status codes (401, 403, 404).
    /// Should be added early in the pipeline to catch all exceptions.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSkywalkerExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SkywalkerExceptionHandlingMiddleware>();
    }

    /// <summary>
    /// Adds unit of work middleware to the pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseUnitOfWork(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UnitOfWorkMiddleware>();
    }
}
