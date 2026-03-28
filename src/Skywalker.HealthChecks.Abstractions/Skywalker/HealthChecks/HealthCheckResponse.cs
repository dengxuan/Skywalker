using System.Text.Json.Serialization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Skywalker.HealthChecks;

/// <summary>
/// Health check response model for API endpoints.
/// </summary>
public class HealthCheckResponse
{
    /// <summary>
    /// Overall health status.
    /// </summary>
    public string Status { get; set; } = "Healthy";

    /// <summary>
    /// Total duration of all health checks.
    /// </summary>
    public TimeSpan TotalDuration { get; set; }

    /// <summary>
    /// Individual health check entries.
    /// </summary>
    public IEnumerable<HealthCheckEntry> Entries { get; set; } = [];
}

/// <summary>
/// Individual health check entry.
/// </summary>
public class HealthCheckEntry
{
    /// <summary>
    /// Name of the health check.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Status of the health check.
    /// </summary>
    public string Status { get; set; } = "Healthy";

    /// <summary>
    /// Duration of the health check.
    /// </summary>
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Description or error message.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Exception details if any.
    /// </summary>
    public string? Exception { get; set; }

    /// <summary>
    /// Additional data from the health check.
    /// </summary>
    public IReadOnlyDictionary<string, object>? Data { get; set; }

    /// <summary>
    /// Tags associated with this health check.
    /// </summary>
    public IEnumerable<string> Tags { get; set; } = [];
}

/// <summary>
/// Simple health check status response.
/// </summary>
public class HealthCheckSimpleResponse
{
    /// <summary>
    /// Overall health status.
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; set; } = "Healthy";

    /// <summary>
    /// Total duration in milliseconds.
    /// </summary>
    [JsonPropertyName("totalDuration")]
    public double TotalDuration { get; set; }
}

/// <summary>
/// Extension methods for creating health check responses.
/// </summary>
public static class HealthCheckResponseExtensions
{
    /// <summary>
    /// Creates a HealthCheckResponse from a HealthReport.
    /// </summary>
    public static HealthCheckResponse ToResponse(this HealthReport report)
    {
        return new HealthCheckResponse
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration,
            Entries = report.Entries.Select(e => new HealthCheckEntry
            {
                Name = e.Key,
                Status = e.Value.Status.ToString(),
                Duration = e.Value.Duration,
                Description = e.Value.Description,
                Exception = e.Value.Exception?.Message,
                Data = e.Value.Data,
                Tags = e.Value.Tags
            })
        };
    }

    /// <summary>
    /// Creates a simple status response from a HealthReport.
    /// </summary>
    public static HealthCheckSimpleResponse ToSimpleResponse(this HealthReport report)
    {
        return new HealthCheckSimpleResponse
        {
            Status = report.Status.ToString(),
            TotalDuration = report.TotalDuration.TotalMilliseconds
        };
    }
}

