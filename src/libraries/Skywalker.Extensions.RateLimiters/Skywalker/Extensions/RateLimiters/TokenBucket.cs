using System;

namespace Skywalker.Extensions.RateLimiters;

public abstract class TokenBucket : IThrottleStrategy
{
    private static readonly object _locker = new();

    protected long TicksRefillInterval { get; }

    protected long BucketTokenCapacity { get; set; }

    protected long NextRefillTime { get; set; }


    /// <summary>
    /// Number of tokens in the bucket
    /// </summary>
    protected long Tokens { get; set; }

    protected TokenBucket(long bucketTokenCapacity, long refillInterval, long refillIntervalInMilliSeconds)
    {
        BucketTokenCapacity = bucketTokenCapacity.Positive(nameof(bucketTokenCapacity));
        refillInterval.Nonnegative(nameof(refillInterval));
        refillIntervalInMilliSeconds.Positive(nameof(refillIntervalInMilliSeconds));

        TicksRefillInterval = TimeSpan.FromMilliseconds(refillInterval * refillIntervalInMilliSeconds).Ticks;
    }

    public bool ShouldThrottle(long n = 1)
    {
        return ShouldThrottle(n, out _);
    }


    public bool ShouldThrottle(long n, out TimeSpan waitTime)
    {
        n.Positive(nameof(n));

        lock (_locker)
        {
            UpdateTokens();
            if (Tokens < n)
            {
                var timeToIntervalEnd = NextRefillTime - DateTime.UtcNow.Ticks;
                if (timeToIntervalEnd < 0)
                {
                    return ShouldThrottle(n, out waitTime);
                }

                waitTime = TimeSpan.FromTicks(timeToIntervalEnd);
                return true;
            }
            Tokens -= n;

            waitTime = TimeSpan.Zero;
            return false;
        }
    }

    protected abstract void UpdateTokens();

    public bool ShouldThrottle(out TimeSpan waitTime)
    {
        return ShouldThrottle(1, out waitTime);
    }

    public long CurrentTokenCount
    {
        get
        {
            lock (_locker)
            {
                UpdateTokens();
                return Tokens;
            }
        }
    }
}
