using System.ComponentModel.DataAnnotations;

namespace Skywalker.Caching.Redis;

/// <summary>
/// Options for Redis caching.
/// </summary>
public class RedisOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Skywalker:Caching:Redis";

    /// <summary>
    /// Gets or sets the Redis connection string.
    /// </summary>
    [Required]
    public string ConnectionString { get; set; } = "127.0.0.1:6379";

    /// <summary>
    /// Gets or sets the instance name prefix for cache keys.
    /// </summary>
    public string? InstanceName { get; set; }

    /// <summary>
    /// Gets or sets the default cache expiration time in minutes.
    /// Default is 60 minutes.
    /// </summary>
    [Range(1, 525600)] // 1 minute to 1 year
    public int DefaultExpirationMinutes { get; set; } = 60;
}
