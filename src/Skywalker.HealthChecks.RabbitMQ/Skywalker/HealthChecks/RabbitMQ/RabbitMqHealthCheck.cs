using Microsoft.Extensions.Diagnostics.HealthChecks;
using Skywalker.Extensions.RabbitMQ.Abstractions;

namespace Skywalker.HealthChecks.RabbitMQ;

/// <summary>
/// Health check for RabbitMQ connection.
/// </summary>
public class RabbitMqHealthCheck : IHealthCheck
{
    private readonly IConnectionPool _connectionPool;

    /// <summary>
    /// Initializes a new instance of <see cref="RabbitMqHealthCheck"/>.
    /// </summary>
    /// <param name="connectionPool">The RabbitMQ connection pool.</param>
    public RabbitMqHealthCheck(IConnectionPool connectionPool)
    {
        _connectionPool = connectionPool;
    }

    /// <inheritdoc />
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var connection = _connectionPool.Get();

            var data = new Dictionary<string, object>
            {
                { "is_open", connection.IsOpen },
                { "endpoint", connection.Endpoint.ToString() },
                { "client_provided_name", connection.ClientProvidedName ?? "N/A" },
                { "server_properties", GetServerProperties(connection) }
            };

            if (!connection.IsOpen)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    description: "RabbitMQ connection is not open",
                    data: data));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                description: $"RabbitMQ connection is healthy. Endpoint: {connection.Endpoint}",
                data: data));
        }
        catch (Exception ex)
        {
            return Task.FromResult(HealthCheckResult.Unhealthy(
                description: "RabbitMQ connection failed",
                exception: ex,
                data: new Dictionary<string, object>
                {
                    { "error", ex.Message }
                }));
        }
    }

    private static Dictionary<string, object> GetServerProperties(global::RabbitMQ.Client.IConnection connection)
    {
        var properties = new Dictionary<string, object>();

        if (connection.ServerProperties.TryGetValue("product", out var product))
        {
            properties["product"] = System.Text.Encoding.UTF8.GetString((byte[])product);
        }

        if (connection.ServerProperties.TryGetValue("version", out var version))
        {
            properties["version"] = System.Text.Encoding.UTF8.GetString((byte[])version);
        }

        return properties;
    }
}

