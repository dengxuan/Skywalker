namespace Skywalker.HealthChecks.AspNetCore;

/// <summary>
/// Options for configuring health check endpoints.
/// </summary>
public class SkywalkerHealthCheckOptions
{
    /// <summary>
    /// Gets or sets the path for the simple health check endpoint.
    /// Default: "/health"
    /// </summary>
    public string HealthEndpoint { get; set; } = HealthCheckConsts.DefaultEndpoint;

    /// <summary>
    /// Gets or sets the path for the detailed health check endpoint.
    /// Default: "/health/detail"
    /// </summary>
    public string DetailedEndpoint { get; set; } = HealthCheckConsts.DetailedEndpoint;

    /// <summary>
    /// Gets or sets the path for the ready check endpoint.
    /// Default: "/health/ready"
    /// </summary>
    public string ReadyEndpoint { get; set; } = HealthCheckConsts.ReadyEndpoint;

    /// <summary>
    /// Gets or sets the path for the live check endpoint.
    /// Default: "/health/live"
    /// </summary>
    public string LiveEndpoint { get; set; } = HealthCheckConsts.LiveEndpoint;

    /// <summary>
    /// Gets or sets whether to enable the detailed endpoint.
    /// Default: true
    /// </summary>
    public bool EnableDetailedEndpoint { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to enable Kubernetes-style ready/live endpoints.
    /// Default: true
    /// </summary>
    public bool EnableKubernetesEndpoints { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to require authorization for detailed endpoint.
    /// Default: false
    /// </summary>
    public bool RequireAuthorizationForDetailedEndpoint { get; set; } = false;

    /// <summary>
    /// Gets or sets the authorization policy name for detailed endpoint.
    /// </summary>
    public string? DetailedEndpointAuthorizationPolicy { get; set; }
}

