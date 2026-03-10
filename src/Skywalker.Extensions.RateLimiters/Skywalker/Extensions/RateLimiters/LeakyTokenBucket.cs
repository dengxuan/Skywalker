namespace Skywalker.Extensions.RateLimiters;

public abstract class LeakyTokenBucket : TokenBucket
{
    protected long StepTokens { get; }

    protected long TicksStepInterval { get; set;  }

    protected LeakyTokenBucket(long maxTokens, long refillInterval, int refillIntervalInMilliSeconds, long stepTokens, long stepInterval, int stepIntervalInMilliseconds)
        : base(maxTokens, refillInterval, refillIntervalInMilliSeconds)
    {
        StepTokens = stepTokens.Nonnegative(nameof(stepTokens));
        stepInterval.Nonnegative(nameof(stepInterval));
        stepIntervalInMilliseconds.Positive(nameof(stepIntervalInMilliseconds));

        TicksStepInterval = TimeSpan.FromMilliseconds(stepInterval * stepIntervalInMilliseconds).Ticks;
    }

}
