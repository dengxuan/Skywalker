// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.RateLimiters.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding rate limiting services.
/// </summary>
public static class RateLimitingServiceCollectionExtensions
{
    /// <summary>
    /// Adds rate limiting services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">The configuration action.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddSkywalkerRateLimiting(
        this IServiceCollection services,
        Action<RateLimitingOptions> configure)
    {
        services.NotNull(nameof(services));
        configure.NotNull(nameof(configure));

        services.Configure(configure);
        return services;
    }
}

