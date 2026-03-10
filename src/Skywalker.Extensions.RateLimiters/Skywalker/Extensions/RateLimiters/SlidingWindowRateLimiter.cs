// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// A rate limiter that uses a sliding time window to limit requests.
/// </summary>
public class SlidingWindowRateLimiter : IRateLimiter
{
    private readonly object _lock = new();
    private readonly SlidingWindowRateLimiterOptions _options;
    private readonly int[] _segmentCounts;
    private long _windowStart;
    private int _currentSegment;
    private int _totalPermitsUsed;
    private long _totalSuccessful;
    private long _totalFailed;

    /// <summary>
    /// Initializes a new instance of the <see cref="SlidingWindowRateLimiter"/> class.
    /// </summary>
    /// <param name="options">The rate limiter options.</param>
    public SlidingWindowRateLimiter(SlidingWindowRateLimiterOptions options)
    {
        _options = options.NotNull(nameof(options));
        _options.PermitLimit.Positive(nameof(options.PermitLimit));
        _options.SegmentsPerWindow.Positive(nameof(options.SegmentsPerWindow));
        
        _segmentCounts = new int[_options.SegmentsPerWindow];
        _windowStart = DateTime.UtcNow.Ticks;
        _currentSegment = 0;
        _totalPermitsUsed = 0;
    }

    /// <inheritdoc />
    public string Name => _options.Name;

    /// <inheritdoc />
    public RateLimitResult TryAcquire(int permitCount = 1)
    {
        permitCount.Positive(nameof(permitCount));

        lock (_lock)
        {
            UpdateSegments();

            var availablePermits = _options.PermitLimit - _totalPermitsUsed;
            if (availablePermits >= permitCount)
            {
                _segmentCounts[_currentSegment] += permitCount;
                _totalPermitsUsed += permitCount;
                Interlocked.Increment(ref _totalSuccessful);
                return RateLimitResult.Success();
            }

            Interlocked.Increment(ref _totalFailed);
            var segmentDuration = _options.Window.Ticks / _options.SegmentsPerWindow;
            var currentTime = DateTime.UtcNow.Ticks;
            var segmentEnd = _windowStart + ((_currentSegment + 1) * segmentDuration);
            var retryAfter = TimeSpan.FromTicks(Math.Max(0, segmentEnd - currentTime));
            
            return RateLimitResult.Failed(retryAfter, "Rate limit exceeded");
        }
    }

    private void UpdateSegments()
    {
        var currentTime = DateTime.UtcNow.Ticks;
        var segmentDuration = _options.Window.Ticks / _options.SegmentsPerWindow;
        var elapsedTicks = currentTime - _windowStart;
        var segmentsPassed = (int)(elapsedTicks / segmentDuration);

        if (segmentsPassed > 0)
        {
            // Move expired segments
            var segmentsToExpire = Math.Min(segmentsPassed, _options.SegmentsPerWindow);
            for (var i = 0; i < segmentsToExpire; i++)
            {
                var expiredSegment = (_currentSegment + 1 + i) % _options.SegmentsPerWindow;
                _totalPermitsUsed -= _segmentCounts[expiredSegment];
                _segmentCounts[expiredSegment] = 0;
            }

            _currentSegment = (_currentSegment + segmentsPassed) % _options.SegmentsPerWindow;
            _windowStart += segmentsPassed * segmentDuration;
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
            UpdateSegments();
            return new RateLimiterStatistics
            {
                CurrentAvailablePermits = _options.PermitLimit - _totalPermitsUsed,
                TotalSuccessfulAcquisitions = Interlocked.Read(ref _totalSuccessful),
                TotalFailedAcquisitions = Interlocked.Read(ref _totalFailed)
            };
        }
    }
}

