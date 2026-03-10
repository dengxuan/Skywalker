using Skywalker.Extensions.RateLimiters;
using Xunit;

namespace Skywalker.Extensions.RateLimiters.Tests;

public class SlidingWindowRateLimiterTests
{
    [Fact]
    public void TryAcquire_WithinLimit_ReturnsSuccess()
    {
        var limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 6
        });

        var result = limiter.TryAcquire();

        Assert.True(result.IsAcquired);
    }

    [Fact]
    public void TryAcquire_ExceedsLimit_ReturnsFailed()
    {
        var limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 2,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 6
        });

        limiter.TryAcquire();
        limiter.TryAcquire();
        var result = limiter.TryAcquire();

        Assert.False(result.IsAcquired);
    }

    [Fact]
    public void TryAcquire_MultiplePermits_DeductsCorrectly()
    {
        var limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 6
        });

        var result = limiter.TryAcquire(5);

        Assert.True(result.IsAcquired);
        var stats = limiter.GetStatistics();
        Assert.Equal(5, stats!.CurrentAvailablePermits);
    }

    [Fact]
    public void GetStatistics_ReturnsCorrectCounts()
    {
        var limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 3,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 6
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
        var limiter = new SlidingWindowRateLimiter(new SlidingWindowRateLimiterOptions
        {
            Name = "sliding-limiter",
            PermitLimit = 10,
            Window = TimeSpan.FromMinutes(1),
            SegmentsPerWindow = 6
        });

        Assert.Equal("sliding-limiter", limiter.Name);
    }
}

