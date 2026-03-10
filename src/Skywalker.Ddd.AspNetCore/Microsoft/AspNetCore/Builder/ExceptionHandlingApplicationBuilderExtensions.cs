using Skywalker.Ddd.AspNetCore.ExceptionHandling;

namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// Extension methods for adding exception handling middleware.
/// </summary>
public static class ExceptionHandlingApplicationBuilderExtensions
{
    /// <summary>
    /// Adds Skywalker exception handling middleware to the pipeline.
    /// This middleware should be added early in the pipeline to catch all exceptions.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseSkywalkerExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SkywalkerExceptionHandlingMiddleware>();
    }
}

