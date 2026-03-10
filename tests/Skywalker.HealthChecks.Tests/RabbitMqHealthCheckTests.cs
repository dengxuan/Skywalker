using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using RabbitMQ.Client;
using Skywalker.Extensions.RabbitMQ.Abstractions;
using Skywalker.HealthChecks.RabbitMQ;
using Xunit;

namespace Skywalker.HealthChecks.Tests;

public class RabbitMqHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenConnectionIsOpen_ReturnsHealthy()
    {
        // Arrange
        var connection = Substitute.For<IConnection>();
        connection.IsOpen.Returns(true);
        connection.Endpoint.Returns(new AmqpTcpEndpoint("localhost", 5672));
        connection.ClientProvidedName.Returns("test-client");
        connection.ServerProperties.Returns(new Dictionary<string, object>
        {
            ["product"] = System.Text.Encoding.UTF8.GetBytes("RabbitMQ"),
            ["version"] = System.Text.Encoding.UTF8.GetBytes("3.12.0")
        });

        var connectionPool = Substitute.For<IConnectionPool>();
        connectionPool.Get(Arg.Any<string?>()).Returns(connection);

        var healthCheck = new RabbitMqHealthCheck(connectionPool);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("rabbitmq", healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains("healthy", result.Description, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(result.Data);
        Assert.True((bool)result.Data["is_open"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenConnectionIsNotOpen_ReturnsUnhealthy()
    {
        // Arrange
        var connection = Substitute.For<IConnection>();
        connection.IsOpen.Returns(false);
        connection.Endpoint.Returns(new AmqpTcpEndpoint("localhost", 5672));
        connection.ServerProperties.Returns(new Dictionary<string, object>());

        var connectionPool = Substitute.For<IConnectionPool>();
        connectionPool.Get(Arg.Any<string?>()).Returns(connection);

        var healthCheck = new RabbitMqHealthCheck(connectionPool);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("rabbitmq", healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Contains("not open", result.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenConnectionFails_ReturnsUnhealthy()
    {
        // Arrange
        var connectionPool = Substitute.For<IConnectionPool>();
        connectionPool.Get(Arg.Any<string?>()).Throws(new Exception("Connection failed"));

        var healthCheck = new RabbitMqHealthCheck(connectionPool);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("rabbitmq", healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Contains("failed", result.Description, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(result.Exception);
    }
}

