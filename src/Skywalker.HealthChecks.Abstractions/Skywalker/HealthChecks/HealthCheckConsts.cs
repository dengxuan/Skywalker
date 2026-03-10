namespace Skywalker.HealthChecks;

/// <summary>
/// Constants for health checks.
/// </summary>
public static class HealthCheckConsts
{
    /// <summary>
    /// Default health check endpoint path.
    /// </summary>
    public const string DefaultEndpoint = "/health";

    /// <summary>
    /// Detailed health check endpoint path.
    /// </summary>
    public const string DetailedEndpoint = "/health/detail";

    /// <summary>
    /// Ready check endpoint path for Kubernetes.
    /// </summary>
    public const string ReadyEndpoint = "/health/ready";

    /// <summary>
    /// Live check endpoint path for Kubernetes.
    /// </summary>
    public const string LiveEndpoint = "/health/live";

    /// <summary>
    /// Default timeout for health checks in seconds.
    /// </summary>
    public const int DefaultTimeoutSeconds = 30;

    /// <summary>
    /// Tag for database health checks.
    /// </summary>
    public const string DatabaseTag = "database";

    /// <summary>
    /// Tag for cache health checks.
    /// </summary>
    public const string CacheTag = "cache";

    /// <summary>
    /// Tag for messaging health checks.
    /// </summary>
    public const string MessagingTag = "messaging";

    /// <summary>
    /// Tag for external service health checks.
    /// </summary>
    public const string ExternalServiceTag = "external";

    /// <summary>
    /// Tag for ready checks.
    /// </summary>
    public const string ReadyTag = "ready";

    /// <summary>
    /// Tag for live checks.
    /// </summary>
    public const string LiveTag = "live";
}

