using Microsoft.Extensions.Diagnostics.HealthChecks;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Skywalker.Caching.Redis.Abstractions;
using Skywalker.HealthChecks.Redis;
using StackExchange.Redis;
using Xunit;

namespace Skywalker.HealthChecks.Tests;

public class RedisHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_WhenRedisIsHealthy_ReturnsHealthy()
    {
        // Arrange
        var database = Substitute.For<IDatabase>();
        var multiplexer = Substitute.For<IConnectionMultiplexer>();
        multiplexer.IsConnected.Returns(true);
        database.Multiplexer.Returns(multiplexer);
        database.Database.Returns(0);
        database.PingAsync(Arg.Any<CommandFlags>()).Returns(TimeSpan.FromMilliseconds(10));

        var provider = Substitute.For<IRedisDatabaseProvider>();
        provider.GetDatabase().Returns(database);

        var healthCheck = new RedisHealthCheck(provider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("redis", healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Contains("healthy", result.Description, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(result.Data);
        Assert.Equal(10.0, result.Data["latency_ms"]);
        Assert.True((bool)result.Data["is_connected"]);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenLatencyIsHigh_ReturnsDegraded()
    {
        // Arrange
        var database = Substitute.For<IDatabase>();
        var multiplexer = Substitute.For<IConnectionMultiplexer>();
        multiplexer.IsConnected.Returns(true);
        database.Multiplexer.Returns(multiplexer);
        database.Database.Returns(0);
        database.PingAsync(Arg.Any<CommandFlags>()).Returns(TimeSpan.FromMilliseconds(1500));

        var provider = Substitute.For<IRedisDatabaseProvider>();
        provider.GetDatabase().Returns(database);

        var healthCheck = new RedisHealthCheck(provider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("redis", healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Degraded, result.Status);
        Assert.Contains("high", result.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CheckHealthAsync_WhenConnectionFails_ReturnsUnhealthy()
    {
        // Arrange
        var provider = Substitute.For<IRedisDatabaseProvider>();
        provider.GetDatabase().Throws(new RedisConnectionException(ConnectionFailureType.UnableToConnect, "Connection failed"));

        var healthCheck = new RedisHealthCheck(provider);
        var context = new HealthCheckContext
        {
            Registration = new HealthCheckRegistration("redis", healthCheck, null, null)
        };

        // Act
        var result = await healthCheck.CheckHealthAsync(context);

        // Assert
        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Contains("failed", result.Description, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(result.Exception);
    }
}

