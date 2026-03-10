// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// Base options for rate limiters.
/// </summary>
public abstract class RateLimiterOptions
{
    /// <summary>
    /// Gets or sets the name of the rate limiter.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum number of permits allowed.
    /// </summary>
    public int PermitLimit { get; set; } = 100;

    /// <summary>
    /// Gets or sets whether to auto-replenish permits.
    /// </summary>
    public bool AutoReplenishment { get; set; } = true;
}

/// <summary>
/// Options for fixed window rate limiter.
/// </summary>
public class FixedWindowRateLimiterOptions : RateLimiterOptions
{
    /// <summary>
    /// Gets or sets the time window duration.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);
}

/// <summary>
/// Options for sliding window rate limiter.
/// </summary>
public class SlidingWindowRateLimiterOptions : RateLimiterOptions
{
    /// <summary>
    /// Gets or sets the time window duration.
    /// </summary>
    public TimeSpan Window { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// Gets or sets the number of segments per window.
    /// </summary>
    public int SegmentsPerWindow { get; set; } = 6;
}

/// <summary>
/// Options for token bucket rate limiter.
/// </summary>
public class TokenBucketRateLimiterOptions : RateLimiterOptions
{
    /// <summary>
    /// Gets or sets the replenishment period.
    /// </summary>
    public TimeSpan ReplenishmentPeriod { get; set; } = TimeSpan.FromSeconds(1);

    /// <summary>
    /// Gets or sets the number of tokens to add per period.
    /// </summary>
    public int TokensPerPeriod { get; set; } = 10;
}

/// <summary>
/// Options for concurrency rate limiter.
/// </summary>
public class ConcurrencyRateLimiterOptions : RateLimiterOptions
{
    /// <summary>
    /// Gets or sets the queue processing order.
    /// </summary>
    public QueueProcessingOrder QueueProcessingOrder { get; set; } = QueueProcessingOrder.OldestFirst;

    /// <summary>
    /// Gets or sets the maximum queue length.
    /// </summary>
    public int QueueLimit { get; set; } = 0;
}

/// <summary>
/// Specifies the order in which queued requests are processed.
/// </summary>
public enum QueueProcessingOrder
{
    /// <summary>
    /// Oldest requests are processed first (FIFO).
    /// </summary>
    OldestFirst,

    /// <summary>
    /// Newest requests are processed first (LIFO).
    /// </summary>
    NewestFirst
}

