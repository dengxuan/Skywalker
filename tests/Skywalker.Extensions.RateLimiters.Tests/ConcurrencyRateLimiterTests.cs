using Skywalker.Extensions.RateLimiters;
using Xunit;

namespace Skywalker.Extensions.RateLimiters.Tests;

public class ConcurrencyRateLimiterTests
{
    [Fact]
    public void TryAcquire_WithinLimit_ReturnsSuccess()
    {
        var limiter = new ConcurrencyRateLimiter(new ConcurrencyRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10
        });

        var result = limiter.TryAcquire();

        Assert.True(result.IsAcquired);
    }

    [Fact]
    public void TryAcquire_ExceedsLimit_ReturnsFailed()
    {
        var limiter = new ConcurrencyRateLimiter(new ConcurrencyRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 2
        });

        limiter.TryAcquire();
        limiter.TryAcquire();
        var result = limiter.TryAcquire();

        Assert.False(result.IsAcquired);
    }

    [Fact]
    public void Release_FreesPermits()
    {
        var limiter = new ConcurrencyRateLimiter(new ConcurrencyRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 2
        });

        limiter.TryAcquire();
        limiter.TryAcquire();
        
        // Should fail - at limit
        Assert.False(limiter.TryAcquire().IsAcquired);
        
        // Release one permit
        limiter.Release();
        
        // Should succeed now
        Assert.True(limiter.TryAcquire().IsAcquired);
    }

    [Fact]
    public void TryAcquire_MultiplePermits_DeductsCorrectly()
    {
        var limiter = new ConcurrencyRateLimiter(new ConcurrencyRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 10
        });

        var result = limiter.TryAcquire(5);

        Assert.True(result.IsAcquired);
        var stats = limiter.GetStatistics();
        Assert.Equal(5, stats!.CurrentAvailablePermits);
    }

    [Fact]
    public void GetStatistics_ReturnsCorrectCounts()
    {
        var limiter = new ConcurrencyRateLimiter(new ConcurrencyRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 3
        });

        limiter.TryAcquire();
        limiter.TryAcquire();
        limiter.TryAcquire();
        limiter.TryAcquire(); // This should fail

        var stats = limiter.GetStatistics();
        Assert.Equal(3, stats!.TotalSuccessfulAcquisitions);
        Assert.Equal(1, stats.TotalFailedAcquisitions);
        Assert.Equal(0, stats.CurrentAvailablePermits);
    }

    [Fact]
    public void Release_MultiplePermits_FreesCorrectly()
    {
        var limiter = new ConcurrencyRateLimiter(new ConcurrencyRateLimiterOptions
        {
            Name = "test",
            PermitLimit = 5
        });

        limiter.TryAcquire(5);
        Assert.Equal(0, limiter.GetStatistics()!.CurrentAvailablePermits);

        limiter.Release(3);
        Assert.Equal(3, limiter.GetStatistics()!.CurrentAvailablePermits);
    }

    [Fact]
    public void Name_ReturnsConfiguredName()
    {
        var limiter = new ConcurrencyRateLimiter(new ConcurrencyRateLimiterOptions
        {
            Name = "concurrency-limiter",
            PermitLimit = 10
        });

        Assert.Equal("concurrency-limiter", limiter.Name);
    }
}

