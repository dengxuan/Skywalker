using Skywalker.HealthChecks;
using Skywalker.HealthChecks.AspNetCore;
using Xunit;

namespace Skywalker.HealthChecks.Tests;

public class SkywalkerHealthCheckOptionsTests
{
    [Fact]
    public void DefaultValues_ShouldBeCorrect()
    {
        var options = new SkywalkerHealthCheckOptions();

        Assert.Equal(HealthCheckConsts.DefaultEndpoint, options.HealthEndpoint);
        Assert.Equal(HealthCheckConsts.DetailedEndpoint, options.DetailedEndpoint);
        Assert.Equal(HealthCheckConsts.ReadyEndpoint, options.ReadyEndpoint);
        Assert.Equal(HealthCheckConsts.LiveEndpoint, options.LiveEndpoint);
        Assert.True(options.EnableDetailedEndpoint);
        Assert.True(options.EnableKubernetesEndpoints);
        Assert.False(options.RequireAuthorizationForDetailedEndpoint);
        Assert.Null(options.DetailedEndpointAuthorizationPolicy);
    }

    [Fact]
    public void Properties_ShouldBeSettable()
    {
        var options = new SkywalkerHealthCheckOptions
        {
            HealthEndpoint = "/custom/health",
            DetailedEndpoint = "/custom/detail",
            ReadyEndpoint = "/custom/ready",
            LiveEndpoint = "/custom/live",
            EnableDetailedEndpoint = false,
            EnableKubernetesEndpoints = false,
            RequireAuthorizationForDetailedEndpoint = true,
            DetailedEndpointAuthorizationPolicy = "AdminOnly"
        };

        Assert.Equal("/custom/health", options.HealthEndpoint);
        Assert.Equal("/custom/detail", options.DetailedEndpoint);
        Assert.Equal("/custom/ready", options.ReadyEndpoint);
        Assert.Equal("/custom/live", options.LiveEndpoint);
        Assert.False(options.EnableDetailedEndpoint);
        Assert.False(options.EnableKubernetesEndpoints);
        Assert.True(options.RequireAuthorizationForDetailedEndpoint);
        Assert.Equal("AdminOnly", options.DetailedEndpointAuthorizationPolicy);
    }
}

