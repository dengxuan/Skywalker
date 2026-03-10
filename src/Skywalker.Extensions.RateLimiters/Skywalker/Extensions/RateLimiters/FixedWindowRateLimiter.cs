// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// A rate limiter that uses a fixed time window to limit requests.
/// </summary>
public class FixedWindowRateLimiter : IRateLimiter
{
    private readonly object _lock = new();
    private readonly FixedWindowRateLimiterOptions _options;
    private long _windowStart;
    private int _permitCount;
    private long _totalSuccessful;
    private long _totalFailed;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedWindowRateLimiter"/> class.
    /// </summary>
    /// <param name="options">The rate limiter options.</param>
    public FixedWindowRateLimiter(FixedWindowRateLimiterOptions options)
    {
        _options = options.NotNull(nameof(options));
        _options.PermitLimit.Positive(nameof(options.PermitLimit));
        
        _windowStart = DateTime.UtcNow.Ticks;
        _permitCount = _options.PermitLimit;
    }

    /// <inheritdoc />
    public string Name => _options.Name;

    /// <inheritdoc />
    public RateLimitResult TryAcquire(int permitCount = 1)
    {
        permitCount.Positive(nameof(permitCount));

        lock (_lock)
        {
            var currentTime = DateTime.UtcNow.Ticks;
            var windowTicks = _options.Window.Ticks;

            // Check if we need to reset the window
            if (currentTime - _windowStart >= windowTicks)
            {
                _windowStart = currentTime;
                _permitCount = _options.PermitLimit;
            }

            if (_permitCount >= permitCount)
            {
                _permitCount -= permitCount;
                Interlocked.Increment(ref _totalSuccessful);
                return RateLimitResult.Success();
            }

            Interlocked.Increment(ref _totalFailed);
            var retryAfter = TimeSpan.FromTicks(_windowStart + windowTicks - currentTime);
            return RateLimitResult.Failed(retryAfter, "Rate limit exceeded");
        }
    }

    /// <inheritdoc />
    public ValueTask<RateLimitResult> TryAcquireAsync(int permitCount = 1, CancellationToken cancellationToken = default)
    {
        return new ValueTask<RateLimitResult>(TryAcquire(permitCount));
    }

    /// <inheritdoc />
    public RateLimiterStatistics? GetStatistics()
    {
        lock (_lock)
        {
            return new RateLimiterStatistics
            {
                CurrentAvailablePermits = _permitCount,
                TotalSuccessfulAcquisitions = Interlocked.Read(ref _totalSuccessful),
                TotalFailedAcquisitions = Interlocked.Read(ref _totalFailed)
            };
        }
    }
}

