using Microsoft.EntityFrameworkCore;
using Skywalker.HealthChecks;
using Skywalker.HealthChecks.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Entity Framework Core database health checks.
/// </summary>
public static class DbContextHealthCheckExtensions
{
    /// <summary>
    /// Adds a health check for an Entity Framework Core DbContext.
    /// </summary>
    /// <typeparam name="TContext">The type of the DbContext.</typeparam>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="name">The name of the health check. Defaults to the context type name.</param>
    /// <param name="failureStatus">The failure status to use.</param>
    /// <param name="tags">Tags for the health check.</param>
    /// <param name="timeout">Timeout for the health check.</param>
    /// <returns>The health checks builder.</returns>
    public static IHealthChecksBuilder AddDbContextHealthCheck<TContext>(
        this IHealthChecksBuilder builder,
        string? name = null,
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
        where TContext : DbContext
    {
        var healthCheckName = name ?? typeof(TContext).Name.Replace("DbContext", string.Empty).ToLowerInvariant();
        var defaultTags = new[] { HealthCheckConsts.DatabaseTag, HealthCheckConsts.ReadyTag };
        var allTags = tags != null ? defaultTags.Concat(tags) : defaultTags;

        return builder.AddCheck<DbContextHealthCheck<TContext>>(
            name: healthCheckName,
            failureStatus: failureStatus,
            tags: allTags,
            timeout: timeout ?? TimeSpan.FromSeconds(HealthCheckConsts.DefaultTimeoutSeconds));
    }
}

