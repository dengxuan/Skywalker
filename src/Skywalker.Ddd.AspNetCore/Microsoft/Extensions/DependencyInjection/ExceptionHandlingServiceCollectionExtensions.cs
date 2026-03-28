using Skywalker.Ddd.AspNetCore.ExceptionHandling;
using Skywalker.Exceptions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding exception handling services.
/// </summary>
public static class ExceptionHandlingServiceCollectionExtensions
{
    /// <summary>
    /// Adds Skywalker exception handling services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSkywalkerExceptionHandling(this IServiceCollection services)
    {
        return services.AddSkywalkerExceptionHandling(_ => { });
    }

    /// <summary>
    /// Adds Skywalker exception handling services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddSkywalkerExceptionHandling(
        this IServiceCollection services,
        Action<ExceptionHandlingOptions> configure)
    {
        services.Configure(configure);

        services.AddSingleton<IHttpExceptionStatusCodeFinder, DefaultHttpExceptionStatusCodeFinder>();
        services.AddSingleton<IExceptionToErrorInfoConverter, DefaultExceptionToErrorInfoConverter>();

        return services;
    }
}

