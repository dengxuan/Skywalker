using Skywalker.Extensions.RateLimiters;
using Xunit;

namespace Skywalker.Extensions.RateLimiters.Tests;

public class FixedWindowRateLimiterTests
{
    [Fact]
    public void TryAcquire_WithinLimit_ReturnsSuccess()
    {
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        var result = limiter.TryAcquire();

        Assert.True(result.IsAcquired);
    }

    [Fact]
    public void TryAcquire_ExceedsLimit_ReturnsFailed()
    {
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 2,
            Window = TimeSpan.FromMinutes(1)
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
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        var result = limiter.TryAcquire(5);

        Assert.True(result.IsAcquired);
        var stats = limiter.GetStatistics();
        Assert.Equal(5, stats!.CurrentAvailablePermits);
    }

    [Fact]
    public void GetStatistics_ReturnsCorrectCounts()
    {
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 3,
            Window = TimeSpan.FromMinutes(1)
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
    public async Task TryAcquireAsync_ReturnsResult()
    {
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        var result = await limiter.TryAcquireAsync();

        Assert.True(result.IsAcquired);
    }

    [Fact]
    public void Name_ReturnsConfiguredName()
    {
        var limiter = new FixedWindowRateLimiter(new FixedWindowRateLimiterOptions
        {
            Name = "my-limiter",
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1)
        });

        Assert.Equal("my-limiter", limiter.Name);
    }
}

