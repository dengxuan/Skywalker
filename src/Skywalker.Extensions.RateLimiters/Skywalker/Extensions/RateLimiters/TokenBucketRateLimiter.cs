// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.RateLimiters;

/// <summary>
/// A rate limiter that uses the token bucket algorithm.
/// </summary>
public class TokenBucketRateLimiter : IRateLimiter
{
    private readonly object _lock = new();
    private readonly TokenBucketRateLimiterOptions _options;
    private double _tokenCount;
    private long _lastReplenishmentTime;
    private long _totalSuccessful;
    private long _totalFailed;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenBucketRateLimiter"/> class.
    /// </summary>
    /// <param name="options">The rate limiter options.</param>
    public TokenBucketRateLimiter(TokenBucketRateLimiterOptions options)
    {
        _options = options.NotNull(nameof(options));
        _options.PermitLimit.Positive(nameof(options.PermitLimit));
        _options.TokensPerPeriod.Positive(nameof(options.TokensPerPeriod));
        
        _tokenCount = _options.PermitLimit;
        _lastReplenishmentTime = DateTime.UtcNow.Ticks;
    }

    /// <inheritdoc />
    public string Name => _options.Name;

    /// <inheritdoc />
    public RateLimitResult TryAcquire(int permitCount = 1)
    {
        permitCount.Positive(nameof(permitCount));

        lock (_lock)
        {
            ReplenishTokens();

            if (_tokenCount >= permitCount)
            {
                _tokenCount -= permitCount;
                Interlocked.Increment(ref _totalSuccessful);
                return RateLimitResult.Success();
            }

            Interlocked.Increment(ref _totalFailed);
            
            // Calculate when enough tokens will be available
            var tokensNeeded = permitCount - _tokenCount;
            var periodsNeeded = Math.Ceiling(tokensNeeded / _options.TokensPerPeriod);
            var retryAfter = TimeSpan.FromTicks((long)(periodsNeeded * _options.ReplenishmentPeriod.Ticks));
            
            return RateLimitResult.Failed(retryAfter, "Rate limit exceeded");
        }
    }

    private void ReplenishTokens()
    {
        if (!_options.AutoReplenishment)
        {
            return;
        }

        var currentTime = DateTime.UtcNow.Ticks;
        var elapsedTicks = currentTime - _lastReplenishmentTime;
        var periodTicks = _options.ReplenishmentPeriod.Ticks;

        if (elapsedTicks >= periodTicks)
        {
            var periodsElapsed = elapsedTicks / periodTicks;
            var tokensToAdd = periodsElapsed * _options.TokensPerPeriod;
            
            _tokenCount = Math.Min(_options.PermitLimit, _tokenCount + tokensToAdd);
            _lastReplenishmentTime = currentTime - (elapsedTicks % periodTicks);
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
            ReplenishTokens();
            return new RateLimiterStatistics
            {
                CurrentAvailablePermits = (long)_tokenCount,
                TotalSuccessfulAcquisitions = Interlocked.Read(ref _totalSuccessful),
                TotalFailedAcquisitions = Interlocked.Read(ref _totalFailed)
            };
        }
    }
}

