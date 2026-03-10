// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// A rate limiter that limits concurrent operations.
/// </summary>
public class ConcurrencyRateLimiter : IRateLimiter
{
    private readonly object _lock = new();
    private readonly ConcurrencyRateLimiterOptions _options;
    private int _currentCount;
    private long _totalSuccessful;
    private long _totalFailed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConcurrencyRateLimiter"/> class.
    /// </summary>
    /// <param name="options">The rate limiter options.</param>
    public ConcurrencyRateLimiter(ConcurrencyRateLimiterOptions options)
    {
        _options = options.NotNull(nameof(options));
        _options.PermitLimit.Positive(nameof(options.PermitLimit));
        _currentCount = 0;
    }

    /// <inheritdoc />
    public string Name => _options.Name;

    /// <inheritdoc />
    public RateLimitResult TryAcquire(int permitCount = 1)
    {
        permitCount.Positive(nameof(permitCount));

        lock (_lock)
        {
            var availablePermits = _options.PermitLimit - _currentCount;
            if (availablePermits >= permitCount)
            {
                _currentCount += permitCount;
                Interlocked.Increment(ref _totalSuccessful);
                return RateLimitResult.Success();
            }

            Interlocked.Increment(ref _totalFailed);
            return RateLimitResult.Failed(TimeSpan.Zero, "Concurrency limit exceeded");
        }
    }

    /// <summary>
    /// Releases the specified number of permits.
    /// </summary>
    /// <param name="permitCount">The number of permits to release.</param>
    public void Release(int permitCount = 1)
    {
        permitCount.Positive(nameof(permitCount));

        lock (_lock)
        {
            _currentCount = Math.Max(0, _currentCount - permitCount);
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
                CurrentAvailablePermits = _options.PermitLimit - _currentCount,
                TotalSuccessfulAcquisitions = Interlocked.Read(ref _totalSuccessful),
                TotalFailedAcquisitions = Interlocked.Read(ref _totalFailed)
            };
        }
    }
}

