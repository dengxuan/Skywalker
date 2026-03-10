using Skywalker.Extensions.RateLimiters;
using Xunit;

namespace Skywalker.Extensions.RateLimiters.Tests;

public class RateLimiterPolicyTests
{
    [Fact]
    public void FixedWindowPolicy_GetRateLimiter_ReturnsSameInstanceForSameKey()
    {
        var policy = new FixedWindowRateLimiterPolicy("test", new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        var limiter1 = policy.GetRateLimiter("key1");
        var limiter2 = policy.GetRateLimiter("key1");

        Assert.Same(limiter1, limiter2);
    }

    [Fact]
    public void FixedWindowPolicy_GetRateLimiter_ReturnsDifferentInstancesForDifferentKeys()
    {
        var policy = new FixedWindowRateLimiterPolicy("test", new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        var limiter1 = policy.GetRateLimiter("key1");
        var limiter2 = policy.GetRateLimiter("key2");

        Assert.NotSame(limiter1, limiter2);
    }

    [Fact]
    public void SlidingWindowPolicy_GetRateLimiter_CreatesLimiter()
    {
        var policy = new SlidingWindowRateLimiterPolicy("test", new SlidingWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 6
        });

        var limiter = policy.GetRateLimiter("key1");

        Assert.NotNull(limiter);
        Assert.Contains("test:key1", limiter.Name);
    }

    [Fact]
    public void TokenBucketPolicy_GetRateLimiter_CreatesLimiter()
    {
        var policy = new TokenBucketRateLimiterPolicy("test", new TokenBucketRateLimiterOptions
        {
            PermitLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 5
        });

        var limiter = policy.GetRateLimiter("key1");

        Assert.NotNull(limiter);
        Assert.Contains("test:key1", limiter.Name);
    }

    [Fact]
    public void Policy_PolicyName_ReturnsConfiguredName()
    {
        var policy = new FixedWindowRateLimiterPolicy("my-policy", new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        Assert.Equal("my-policy", policy.PolicyName);
    }

    [Fact]
    public async Task Policy_OnRejectedAsync_CompletesSuccessfully()
    {
        var policy = new FixedWindowRateLimiterPolicy("test", new FixedWindowRateLimiterOptions
        {
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        await policy.OnRejectedAsync("key1", RateLimitResult.Failed(TimeSpan.FromSeconds(10)));
        // Should complete without exception
    }
}

