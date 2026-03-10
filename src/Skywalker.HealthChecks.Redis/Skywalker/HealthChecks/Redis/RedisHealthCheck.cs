using Microsoft.Extensions.Diagnostics.HealthChecks;
using Skywalker.Caching.Redis.Abstractions;

namespace Skywalker.HealthChecks.Redis;

/// <summary>
/// Health check for Redis cache connection.
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IRedisDatabaseProvider _redisDatabaseProvider;

    /// <summary>
    /// Initializes a new instance of <see cref="RedisHealthCheck"/>.
    /// </summary>
    /// <param name="redisDatabaseProvider">The Redis database provider.</param>
    public RedisHealthCheck(IRedisDatabaseProvider redisDatabaseProvider)
    {
        _redisDatabaseProvider = redisDatabaseProvider;
    }

    /// <inheritdoc />
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var database = _redisDatabaseProvider.GetDatabase();
            var pingResult = await database.PingAsync();

            var data = new Dictionary<string, object>
            {
                { "latency_ms", pingResult.TotalMilliseconds },
                { "database_number", database.Database },
                { "is_connected", database.Multiplexer.IsConnected }
            };

            if (pingResult.TotalMilliseconds > 1000)
            {
                return HealthCheckResult.Degraded(
                    description: $"Redis ping latency is high: {pingResult.TotalMilliseconds}ms",
                    data: data);
            }

            return HealthCheckResult.Healthy(
                description: $"Redis is healthy. Ping: {pingResult.TotalMilliseconds}ms",
                data: data);
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                description: "Redis connection failed",
                exception: ex,
                data: new Dictionary<string, object>
                {
                    { "error", ex.Message }
                });
        }
    }
}

