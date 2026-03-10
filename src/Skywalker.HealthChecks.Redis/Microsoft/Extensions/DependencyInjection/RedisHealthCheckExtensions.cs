using Skywalker.HealthChecks;
using Skywalker.HealthChecks.Redis;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding Redis health checks.
/// </summary>
public static class RedisHealthCheckExtensions
{
    /// <summary>
    /// Adds a health check for Redis cache.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="name">The name of the health check.</param>
    /// <param name="failureStatus">The failure status to use.</param>
    /// <param name="tags">Tags for the health check.</param>
    /// <param name="timeout">Timeout for the health check.</param>
    /// <returns>The health checks builder.</returns>
    public static IHealthChecksBuilder AddRedisHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "redis",
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
    {
        var defaultTags = new[] { HealthCheckConsts.CacheTag, HealthCheckConsts.ReadyTag };
        var allTags = tags != null ? defaultTags.Concat(tags) : defaultTags;

        return builder.AddCheck<RedisHealthCheck>(
            name: name,
            failureStatus: failureStatus,
            tags: allTags,
            timeout: timeout ?? TimeSpan.FromSeconds(HealthCheckConsts.DefaultTimeoutSeconds));
    }
}

