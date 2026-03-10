using Skywalker.HealthChecks.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Skywalker health checks to the service collection.
/// </summary>
public static class HealthCheckServiceCollectionExtensions
{
    /// <summary>
    /// Adds Skywalker health check services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The health checks builder.</returns>
    public static IHealthChecksBuilder AddSkywalkerHealthChecks(
        this IServiceCollection services,
        Action<SkywalkerHealthCheckOptions>? configure = null)
    {
        var options = new SkywalkerHealthCheckOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);

        return services.AddHealthChecks();
    }
}

