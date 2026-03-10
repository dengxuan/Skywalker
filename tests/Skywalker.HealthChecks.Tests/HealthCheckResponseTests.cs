using Microsoft.Extensions.Diagnostics.HealthChecks;
using Skywalker.HealthChecks;
using Xunit;

namespace Skywalker.HealthChecks.Tests;

public class HealthCheckResponseTests
{
    [Fact]
    public void HealthCheckResponse_DefaultValues_ShouldBeCorrect()
    {
        var response = new HealthCheckResponse();

        Assert.Equal("Healthy", response.Status);
        Assert.Equal(TimeSpan.Zero, response.TotalDuration);
        Assert.Empty(response.Entries);
    }

    [Fact]
    public void HealthCheckEntry_DefaultValues_ShouldBeCorrect()
    {
        var entry = new HealthCheckEntry();

        Assert.Equal(string.Empty, entry.Name);
        Assert.Equal("Healthy", entry.Status);
        Assert.Equal(TimeSpan.Zero, entry.Duration);
        Assert.Null(entry.Description);
        Assert.Null(entry.Exception);
        Assert.Null(entry.Data);
        Assert.Empty(entry.Tags);
    }

    [Fact]
    public void ToResponse_ShouldConvertHealthReport()
    {
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(
                status: HealthStatus.Healthy,
                description: "Test is healthy",
                duration: TimeSpan.FromMilliseconds(100),
                exception: null,
                data: new Dictionary<string, object> { ["key"] = "value" },
                tags: ["tag1", "tag2"])
        };

        var report = new HealthReport(entries, TimeSpan.FromMilliseconds(150));

        var response = report.ToResponse();

        Assert.Equal("Healthy", response.Status);
        Assert.Equal(TimeSpan.FromMilliseconds(150), response.TotalDuration);
        Assert.Single(response.Entries);

        var entry = response.Entries.First();
        Assert.Equal("test", entry.Name);
        Assert.Equal("Healthy", entry.Status);
        Assert.Equal(TimeSpan.FromMilliseconds(100), entry.Duration);
        Assert.Equal("Test is healthy", entry.Description);
        Assert.Null(entry.Exception);
        Assert.NotNull(entry.Data);
        Assert.Equal("value", entry.Data["key"]);
        Assert.Contains("tag1", entry.Tags);
        Assert.Contains("tag2", entry.Tags);
    }

    [Fact]
    public void ToSimpleResponse_ShouldReturnSimpleObject()
    {
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(
                status: HealthStatus.Healthy,
                description: null,
                duration: TimeSpan.FromMilliseconds(100),
                exception: null,
                data: null,
                tags: [])
        };

        var report = new HealthReport(entries, TimeSpan.FromMilliseconds(150));

        var response = report.ToSimpleResponse();

        Assert.NotNull(response);
        var type = response.GetType();
        var statusProperty = type.GetProperty("status");
        var durationProperty = type.GetProperty("totalDuration");

        Assert.NotNull(statusProperty);
        Assert.NotNull(durationProperty);
        Assert.Equal("Healthy", statusProperty.GetValue(response));
        Assert.Equal(150.0, durationProperty.GetValue(response));
    }

    [Theory]
    [InlineData(HealthStatus.Healthy, "Healthy")]
    [InlineData(HealthStatus.Degraded, "Degraded")]
    [InlineData(HealthStatus.Unhealthy, "Unhealthy")]
    public void ToResponse_ShouldMapStatusCorrectly(HealthStatus status, string expectedStatus)
    {
        var entries = new Dictionary<string, HealthReportEntry>
        {
            ["test"] = new HealthReportEntry(
                status: status,
                description: null,
                duration: TimeSpan.Zero,
                exception: null,
                data: null,
                tags: [])
        };

        var report = new HealthReport(entries, TimeSpan.Zero);

        var response = report.ToResponse();

        Assert.Equal(expectedStatus, response.Status);
    }
}

