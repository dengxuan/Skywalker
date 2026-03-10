using Skywalker.Extensions.RateLimiters;
using Xunit;

namespace Skywalker.Extensions.RateLimiters.Tests;

public class TokenBucketRateLimiterTests
{
    [Fact]
    public void TryAcquire_WithinLimit_ReturnsSuccess()
    {
        var limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 5
        });

        var result = limiter.TryAcquire();

        Assert.True(result.IsAcquired);
    }

    [Fact]
    public void TryAcquire_ExceedsLimit_ReturnsFailed()
    {
        var limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 2,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 1,
            AutoReplenishment = false
        });

        limiter.TryAcquire();
        limiter.TryAcquire();
        var result = limiter.TryAcquire();

        Assert.False(result.IsAcquired);
        Assert.True(result.RetryAfter > TimeSpan.Zero);
    }

    [Fact]
    public void TryAcquire_MultiplePermits_DeductsCorrectly()
    {
        var limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 5,
            AutoReplenishment = false
        });

        var result = limiter.TryAcquire(5);

        Assert.True(result.IsAcquired);
        var stats = limiter.GetStatistics();
        Assert.Equal(5, stats!.CurrentAvailablePermits);
    }

    [Fact]
    public void GetStatistics_ReturnsCorrectCounts()
    {
        var limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 3,
            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
            TokensPerPeriod = 1,
            AutoReplenishment = false
        });

        limiter.TryAcquire();
        limiter.TryAcquire();
        limiter.TryAcquire();
        limiter.TryAcquire(); // This should fail

        var stats = limiter.GetStatistics();
        Assert.Equal(3, stats!.TotalSuccessfulAcquisitions);
        Assert.Equal(1, stats.TotalFailedAcquisitions);
    }

    [Fact]
    public void Name_ReturnsConfiguredName()
    {
        var limiter = new TokenBucketRateLimiter(new TokenBucketRateLimiterOptions
        {
            Name = "token-bucket",
            PermitLimit = 10,
            ReplenishmentPeriod = TimeSpan.FromSeconds(1),
            TokensPerPeriod = 5
        });

        Assert.Equal("token-bucket", limiter.Name);
    }
}

