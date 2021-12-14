using System;

namespace Skywalker.Extensions.RateLimiters;

public class FixedTokenBucket : TokenBucket
{
    public FixedTokenBucket(long maxTokens, long refillInterval, long refillIntervalInMilliSeconds) : base(maxTokens, refillInterval, refillIntervalInMilliSeconds)
    {
    }

    protected override void UpdateTokens()
    {
        var currentTime = DateTime.UtcNow.Ticks;

        if (currentTime < NextRefillTime)
        {
            return;
        }

        Tokens = BucketTokenCapacity;
        NextRefillTime = currentTime + TicksRefillInterval;
    }
}
