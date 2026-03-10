// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// Represents a rate limiter that controls the rate of operations.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Gets the name of this rate limiter.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Attempts to acquire a permit.
    /// </summary>
    /// <param name="permitCount">Number of permits to acquire.</param>
    /// <returns>A result indicating whether the permit was acquired.</returns>
    RateLimitResult TryAcquire(int permitCount = 1);

    /// <summary>
    /// Attempts to acquire a permit asynchronously.
    /// </summary>
    /// <param name="permitCount">Number of permits to acquire.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result indicating whether the permit was acquired.</returns>
    ValueTask<RateLimitResult> TryAcquireAsync(int permitCount = 1, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current statistics of the rate limiter.
    /// </summary>
    RateLimiterStatistics? GetStatistics();
}

/// <summary>
/// Result of a rate limit acquisition attempt.
/// </summary>
public readonly struct RateLimitResult
{
    /// <summary>
    /// Gets a value indicating whether the acquisition was successful.
    /// </summary>
    public bool IsAcquired { get; }

    /// <summary>
    /// Gets the retry-after time if the acquisition failed.
    /// </summary>
    public TimeSpan RetryAfter { get; }

    /// <summary>
    /// Gets the reason for rejection if acquisition failed.
    /// </summary>
    public string? Reason { get; }

    private RateLimitResult(bool isAcquired, TimeSpan retryAfter, string? reason)
    {
        IsAcquired = isAcquired;
        RetryAfter = retryAfter;
        Reason = reason;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static RateLimitResult Success() => new(true, TimeSpan.Zero, null);

    /// <summary>
    /// Creates a failed result with retry-after time.
    /// </summary>
    public static RateLimitResult Failed(TimeSpan retryAfter, string? reason = null) 
        => new(false, retryAfter, reason);
}

/// <summary>
/// Statistics for a rate limiter.
/// </summary>
public class RateLimiterStatistics
{
    /// <summary>
    /// Gets or sets the current available permits.
    /// </summary>
    public long CurrentAvailablePermits { get; init; }

    /// <summary>
    /// Gets or sets the total successful acquisitions.
    /// </summary>
    public long TotalSuccessfulAcquisitions { get; init; }

    /// <summary>
    /// Gets or sets the total failed acquisitions.
    /// </summary>
    public long TotalFailedAcquisitions { get; init; }
}

