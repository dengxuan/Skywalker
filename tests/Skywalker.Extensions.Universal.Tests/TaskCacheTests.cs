using System.Threading.Tasks;
using Xunit;

namespace Skywalker.Extensions.Universal.Tests;

public class TaskCacheTests
{
    [Fact]
    public async Task TrueResult_ReturnsCompletedTaskWithTrue()
    {
        var task = TaskCache.TrueResult;

        Assert.NotNull(task);
        Assert.True(task.IsCompleted);
        Assert.True(await task);
    }

    [Fact]
    public async Task FalseResult_ReturnsCompletedTaskWithFalse()
    {
        var task = TaskCache.FalseResult;

        Assert.NotNull(task);
        Assert.True(task.IsCompleted);
        Assert.False(await task);
    }

    [Fact]
    public void TrueResult_ReturnsSameInstance()
    {
        var task1 = TaskCache.TrueResult;
        var task2 = TaskCache.TrueResult;

        Assert.Same(task1, task2);
    }

    [Fact]
    public void FalseResult_ReturnsSameInstance()
    {
        var task1 = TaskCache.FalseResult;
        var task2 = TaskCache.FalseResult;

        Assert.Same(task1, task2);
    }

    [Fact]
    public async Task TrueResult_DifferentFromFalseResult()
    {
        var trueTask = TaskCache.TrueResult;
        var falseTask = TaskCache.FalseResult;

        Assert.NotSame(trueTask, falseTask);
        Assert.NotEqual(await trueTask, await falseTask);
    }
}
