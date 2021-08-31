namespace Skywalker.Extensions.RateLimiters;

public class Throttler
{
    private readonly IThrottleStrategy _strategy;

    public Throttler(IThrottleStrategy strategy)
    {
        _strategy = strategy.NotNull(nameof(strategy));
    }

    public bool CanConsume()
    {
        return !_strategy.ShouldThrottle();
    }
}
