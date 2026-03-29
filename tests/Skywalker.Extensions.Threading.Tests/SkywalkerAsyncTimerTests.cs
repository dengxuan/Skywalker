using Microsoft.Extensions.Logging;
using NSubstitute;
using Skywalker.Extensions.Threading;

namespace Skywalker.Extensions.Threading.Tests;

public class SkywalkerAsyncTimerTests
{
    private readonly ILogger<SkywalkerAsyncTimer> _logger;

    public SkywalkerAsyncTimerTests()
    {
        _logger = Substitute.For<ILogger<SkywalkerAsyncTimer>>();
    }

    [Fact]
    public void Start_ThrowsIfPeriodNotSet()
    {
        var timer = new SkywalkerAsyncTimer(_logger);

        Assert.Throws<ArgumentException>(() => timer.Start());
    }

    [Fact]
    public void Start_ThrowsIfPeriodIsZero()
    {
        var timer = new SkywalkerAsyncTimer(_logger);
        timer.Period = 0;

        Assert.Throws<ArgumentException>(() => timer.Start());
    }

    [Fact]
    public void Start_ThrowsIfPeriodIsNegative()
    {
        var timer = new SkywalkerAsyncTimer(_logger);
        timer.Period = -1;

        Assert.Throws<ArgumentException>(() => timer.Start());
    }

    [Fact]
    public async Task Timer_InvokesElapsed_AfterStart()
    {
        var timer = new SkywalkerAsyncTimer(_logger);
        timer.Period = 50;
        timer.RunOnStart = true;

        var tcs = new TaskCompletionSource<bool>();
        timer.Elapsed = _ =>
        {
            tcs.TrySetResult(true);
            return Task.CompletedTask;
        };

        timer.Start();
        var fired = await Task.WhenAny(tcs.Task, Task.Delay(5000)) == tcs.Task;
        timer.Stop();

        Assert.True(fired, "Timer should have fired at least once");
    }

    [Fact]
    public async Task Timer_InvokesElapsed_MultipleTimes()
    {
        var timer = new SkywalkerAsyncTimer(_logger);
        timer.Period = 50;
        timer.RunOnStart = true;

        var count = 0;
        var tcs = new TaskCompletionSource<bool>();
        timer.Elapsed = _ =>
        {
            if (Interlocked.Increment(ref count) >= 3)
            {
                tcs.TrySetResult(true);
            }
            return Task.CompletedTask;
        };

        timer.Start();
        var fired = await Task.WhenAny(tcs.Task, Task.Delay(5000)) == tcs.Task;
        timer.Stop();

        Assert.True(fired, "Timer should have fired at least 3 times");
        Assert.True(count >= 3);
    }

    [Fact]
    public async Task Timer_DoesNotOverlap_WhenElapsedIsSlow()
    {
        var timer = new SkywalkerAsyncTimer(_logger);
        timer.Period = 50;
        timer.RunOnStart = true;

        var concurrentCount = 0;
        var maxConcurrent = 0;
        var invocations = 0;
        var tcs = new TaskCompletionSource<bool>();

        timer.Elapsed = async _ =>
        {
            var current = Interlocked.Increment(ref concurrentCount);
            if (current > Interlocked.CompareExchange(ref maxConcurrent, 0, 0))
            {
                Interlocked.Exchange(ref maxConcurrent, current);
            }
            await Task.Delay(100);
            Interlocked.Decrement(ref concurrentCount);
            if (Interlocked.Increment(ref invocations) >= 3)
            {
                tcs.TrySetResult(true);
            }
        };

        timer.Start();
        await Task.WhenAny(tcs.Task, Task.Delay(5000));
        timer.Stop();

        Assert.Equal(1, maxConcurrent);
    }

    [Fact]
    public async Task Stop_WaitsForCurrentExecution()
    {
        var timer = new SkywalkerAsyncTimer(_logger);
        timer.Period = 50;
        timer.RunOnStart = true;

        var executing = false;
        var completed = false;

        timer.Elapsed = async _ =>
        {
            executing = true;
            await Task.Delay(200);
            completed = true;
        };

        timer.Start();
        // Wait for task to start
        var timeout = DateTime.UtcNow.AddSeconds(5);
        while (!executing && DateTime.UtcNow < timeout)
        {
            await Task.Delay(10);
        }

        timer.Stop();
        Assert.True(completed, "Stop should wait for the current execution to complete");
    }

    [Fact]
    public async Task Timer_HandlesExceptions_Gracefully()
    {
        var timer = new SkywalkerAsyncTimer(_logger);
        timer.Period = 50;
        timer.RunOnStart = true;

        var callCount = 0;
        var tcs = new TaskCompletionSource<bool>();

        timer.Elapsed = _ =>
        {
            var count = Interlocked.Increment(ref callCount);
            if (count == 1)
            {
                throw new InvalidOperationException("Test exception");
            }
            if (count >= 2)
            {
                tcs.TrySetResult(true);
            }
            return Task.CompletedTask;
        };

        timer.Start();
        var continued = await Task.WhenAny(tcs.Task, Task.Delay(5000)) == tcs.Task;
        timer.Stop();

        Assert.True(continued, "Timer should continue running after exception");
    }

    [Fact]
    public void DefaultProperties_AreCorrect()
    {
        var timer = new SkywalkerAsyncTimer(_logger);

        Assert.Equal(0, timer.Period);
        Assert.False(timer.RunOnStart);
    }
}
