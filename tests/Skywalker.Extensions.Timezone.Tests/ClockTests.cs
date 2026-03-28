using Microsoft.Extensions.Options;
using Skywalker.Extensions.Timezone;

namespace Skywalker.Extensions.Timezone.Tests;

public class ClockTests
{
    private Clock CreateClock(DateTimeKind kind)
    {
        var options = Options.Create(new TimezoneOptions { Kind = kind });
        return new Clock(options);
    }

    [Fact]
    public void Kind_ShouldMatchOptions()
    {
        var clock = CreateClock(DateTimeKind.Utc);
        Assert.Equal(DateTimeKind.Utc, clock.Kind);
    }

    [Fact]
    public void SupportsMultipleTimezone_Utc_ShouldReturnTrue()
    {
        var clock = CreateClock(DateTimeKind.Utc);
        Assert.True(clock.SupportsMultipleTimezone);
    }

    [Fact]
    public void SupportsMultipleTimezone_Local_ShouldReturnFalse()
    {
        var clock = CreateClock(DateTimeKind.Local);
        Assert.False(clock.SupportsMultipleTimezone);
    }

    [Fact]
    public void Now_Utc_ShouldReturnUtcNow()
    {
        var clock = CreateClock(DateTimeKind.Utc);
        var now = clock.Now;
        Assert.Equal(DateTimeKind.Utc, now.Kind);
    }

    [Fact]
    public void Now_Local_ShouldReturnLocalNow()
    {
        var clock = CreateClock(DateTimeKind.Local);
        var now = clock.Now;
        Assert.Equal(DateTimeKind.Local, now.Kind);
    }

    [Fact]
    public void Normalize_SameKind_ShouldReturnSame()
    {
        var clock = CreateClock(DateTimeKind.Utc);
        var dt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var result = clock.Normalize(dt);
        Assert.Equal(dt, result);
    }

    [Fact]
    public void Normalize_UnspecifiedKind_ShouldReturnSame()
    {
        var clock = CreateClock(DateTimeKind.Unspecified);
        var dt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var result = clock.Normalize(dt);
        Assert.Equal(dt, result);
    }

    [Fact]
    public void Normalize_UtcToLocal_ShouldConvert()
    {
        var clock = CreateClock(DateTimeKind.Local);
        var utcDt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var result = clock.Normalize(utcDt);
        Assert.Equal(DateTimeKind.Local, result.Kind);
        Assert.Equal(utcDt.ToLocalTime(), result);
    }

    [Fact]
    public void Normalize_LocalToUtc_ShouldConvert()
    {
        var clock = CreateClock(DateTimeKind.Utc);
        var localDt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Local);
        var result = clock.Normalize(localDt);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
        Assert.Equal(localDt.ToUniversalTime(), result);
    }

    [Fact]
    public void Normalize_UnspecifiedInputToUtc_ShouldSpecifyKind()
    {
        var clock = CreateClock(DateTimeKind.Utc);
        var dt = new DateTime(2024, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
        var result = clock.Normalize(dt);
        Assert.Equal(DateTimeKind.Utc, result.Kind);
    }
}

public class TimezoneOptionsTests
{
    [Fact]
    public void DefaultKind_ShouldBeUnspecified()
    {
        var options = new TimezoneOptions();
        Assert.Equal(DateTimeKind.Unspecified, options.Kind);
    }

    [Fact]
    public void Kind_ShouldBeSettable()
    {
        var options = new TimezoneOptions { Kind = DateTimeKind.Utc };
        Assert.Equal(DateTimeKind.Utc, options.Kind);
    }
}
