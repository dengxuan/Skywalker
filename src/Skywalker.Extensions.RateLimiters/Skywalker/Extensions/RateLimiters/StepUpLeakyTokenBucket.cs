using System;

namespace Skywalker.Extensions.RateLimiters;

public class StepUpLeakyTokenBucket : LeakyTokenBucket
{
    private long lastActivityTime;

    public StepUpLeakyTokenBucket(long maxTokens, long refillInterval, int refillIntervalInMilliSeconds, long stepTokens, long stepInterval, int stepIntervalInMilliseconds) : base(maxTokens, refillInterval, refillIntervalInMilliSeconds, stepTokens, stepInterval, stepIntervalInMilliseconds)
    {
    }

    protected override void UpdateTokens()
    {
        var currentTime = DateTime.UtcNow.Ticks;

        if (currentTime >= NextRefillTime)
        {
            Tokens = StepTokens;

            lastActivityTime = currentTime;
            NextRefillTime = currentTime + TicksRefillInterval;

            return;
        }

        //calculate tokens at current step

        long elapsedTimeSinceLastActivity = currentTime - lastActivityTime;
        long elapsedStepsSinceLastActivity = elapsedTimeSinceLastActivity / TicksStepInterval;

        Tokens += elapsedStepsSinceLastActivity * StepTokens;

        if (Tokens > BucketTokenCapacity) Tokens = BucketTokenCapacity;
        lastActivityTime = currentTime;
    }
}
