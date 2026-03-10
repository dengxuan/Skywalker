using Skywalker.HealthChecks;
using Skywalker.HealthChecks.RabbitMQ;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for adding RabbitMQ health checks.
/// </summary>
public static class RabbitMqHealthCheckExtensions
{
    /// <summary>
    /// Adds a health check for RabbitMQ connection.
    /// </summary>
    /// <param name="builder">The health checks builder.</param>
    /// <param name="name">The name of the health check.</param>
    /// <param name="failureStatus">The failure status to use.</param>
    /// <param name="tags">Tags for the health check.</param>
    /// <param name="timeout">Timeout for the health check.</param>
    /// <returns>The health checks builder.</returns>
    public static IHealthChecksBuilder AddRabbitMqHealthCheck(
        this IHealthChecksBuilder builder,
        string name = "rabbitmq",
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null,
        TimeSpan? timeout = null)
    {
        var defaultTags = new[] { HealthCheckConsts.MessagingTag, HealthCheckConsts.ReadyTag };
        var allTags = tags != null ? defaultTags.Concat(tags) : defaultTags;

        return builder.AddCheck<RabbitMqHealthCheck>(
            name: name,
            failureStatus: failureStatus,
            tags: allTags,
            timeout: timeout ?? TimeSpan.FromSeconds(HealthCheckConsts.DefaultTimeoutSeconds));
    }
}

