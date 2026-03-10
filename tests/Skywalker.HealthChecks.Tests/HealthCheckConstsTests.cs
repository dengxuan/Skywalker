using Skywalker.HealthChecks;
using Xunit;

namespace Skywalker.HealthChecks.Tests;

public class HealthCheckConstsTests
{
    [Fact]
    public void DefaultEndpoint_ShouldBeHealth()
    {
        Assert.Equal("/health", HealthCheckConsts.DefaultEndpoint);
    }

    [Fact]
    public void DetailedEndpoint_ShouldBeHealthDetail()
    {
        Assert.Equal("/health/detail", HealthCheckConsts.DetailedEndpoint);
    }

    [Fact]
    public void ReadyEndpoint_ShouldBeHealthReady()
    {
        Assert.Equal("/health/ready", HealthCheckConsts.ReadyEndpoint);
    }

    [Fact]
    public void LiveEndpoint_ShouldBeHealthLive()
    {
        Assert.Equal("/health/live", HealthCheckConsts.LiveEndpoint);
    }

    [Fact]
    public void DefaultTimeoutSeconds_ShouldBe30()
    {
        Assert.Equal(30, HealthCheckConsts.DefaultTimeoutSeconds);
    }

    [Fact]
    public void Tags_ShouldHaveCorrectValues()
    {
        Assert.Equal("database", HealthCheckConsts.DatabaseTag);
        Assert.Equal("cache", HealthCheckConsts.CacheTag);
        Assert.Equal("messaging", HealthCheckConsts.MessagingTag);
        Assert.Equal("external", HealthCheckConsts.ExternalServiceTag);
        Assert.Equal("ready", HealthCheckConsts.ReadyTag);
        Assert.Equal("live", HealthCheckConsts.LiveTag);
    }
}

