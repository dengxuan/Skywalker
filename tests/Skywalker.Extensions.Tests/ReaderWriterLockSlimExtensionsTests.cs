using Xunit;

namespace Skywalker.Extensions.Tests;

public class ReaderWriterLockSlimExtensionsTests
{
    // ReadLocking - Action
    [Fact]
    public void ReadLocking_Action_ExecutesInsideLock()
    {
        var locker = new ReaderWriterLockSlim();
        var executed = false;

        locker.ReadLocking(() => executed = true);

        Assert.True(executed);
        Assert.False(locker.IsReadLockHeld);
    }

    [Fact]
    public void ReadLocking_ActionWithParam_ExecutesWithObject()
    {
        var locker = new ReaderWriterLockSlim();
        string? captured = null;

        locker.ReadLocking<string>(s => captured = s, "test");

        Assert.Equal("test", captured);
    }

    // ReadLocking - Func<TResult>
    [Fact]
    public void ReadLocking_Func_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = locker.ReadLocking(() => 42);

        Assert.Equal(42, result);
        Assert.False(locker.IsReadLockHeld);
    }

    [Fact]
    public void ReadLocking_FuncWithParam_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = locker.ReadLocking<string, int>(s => s!.Length, "hello");

        Assert.Equal(5, result);
    }

    // ReadLockingAsync
    [Fact]
    public async Task ReadLockingAsync_Action_ExecutesInsideLock()
    {
        var locker = new ReaderWriterLockSlim();
        var executed = false;

        await locker.ReadLockingAsync(() => executed = true);

        Assert.True(executed);
    }

    [Fact]
    public async Task ReadLockingAsync_ActionWithParam_ExecutesWithObject()
    {
        var locker = new ReaderWriterLockSlim();
        string? captured = null;

        await locker.ReadLockingAsync<string>(s => captured = s, "async-test");

        Assert.Equal("async-test", captured);
    }

    [Fact]
    public async Task ReadLockingAsync_Func_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = await locker.ReadLockingAsync(() => Task.FromResult(99));

        Assert.Equal(99, result);
    }

    [Fact]
    public async Task ReadLockingAsync_FuncWithParam_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = await locker.ReadLockingAsync<string, int>(
            s => Task.FromResult(s!.Length), "hello");

        Assert.Equal(5, result);
    }

    // WriteLocking - Action
    [Fact]
    public void WriteLocking_Action_ExecutesInsideLock()
    {
        var locker = new ReaderWriterLockSlim();
        var executed = false;

        locker.WriteLocking(() => executed = true);

        Assert.True(executed);
        Assert.False(locker.IsWriteLockHeld);
    }

    [Fact]
    public void WriteLocking_ActionWithParam_ExecutesWithObject()
    {
        var locker = new ReaderWriterLockSlim();
        int captured = 0;

        locker.WriteLocking<int>(i => captured = i, 42);

        Assert.Equal(42, captured);
    }

    // WriteLocking - Func<TResult>
    [Fact]
    public void WriteLocking_Func_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = locker.WriteLocking(() => "written");

        Assert.Equal("written", result);
        Assert.False(locker.IsWriteLockHeld);
    }

    [Fact]
    public void WriteLocking_FuncWithParam_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = locker.WriteLocking<int, string>(i => $"value={i}", 42);

        Assert.Equal("value=42", result);
    }

    // WriteLockingAsync
    [Fact]
    public async Task WriteLockingAsync_Action_ExecutesInsideLock()
    {
        var locker = new ReaderWriterLockSlim();
        var executed = false;

        await locker.WriteLockingAsync(() => executed = true);

        Assert.True(executed);
    }

    [Fact]
    public async Task WriteLockingAsync_ActionWithParam_ExecutesWithObject()
    {
        var locker = new ReaderWriterLockSlim();
        string? captured = null;

        await locker.WriteLockingAsync<string>(s => captured = s, "async-write");

        Assert.Equal("async-write", captured);
    }

    [Fact]
    public async Task WriteLockingAsync_Func_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = await locker.WriteLockingAsync(() => Task.FromResult(77));

        Assert.Equal(77, result);
    }

    [Fact]
    public async Task WriteLockingAsync_FuncWithParam_ReturnsResult()
    {
        var locker = new ReaderWriterLockSlim();

        var result = await locker.WriteLockingAsync<string, int>(
            s => Task.FromResult(s!.Length), "write");

        Assert.Equal(5, result);
    }

    // ReadWriteLocking
    [Fact]
    public void ReadWriteLocking_SharedReturnsValue_ReturnsShared()
    {
        var locker = new ReaderWriterLockSlim();

        var result = locker.ReadWriteLocking(
            () => "from-shared",
            () => "from-exclusive");

        Assert.Equal("from-shared", result);
    }

    [Fact]
    public void ReadWriteLocking_SharedReturnsNull_ReturnsExclusive()
    {
        var locker = new ReaderWriterLockSlim();

        var result = locker.ReadWriteLocking<string>(
            () => null!,
            () => "from-exclusive");

        Assert.Equal("from-exclusive", result);
    }

    [Fact]
    public void ReadWriteLocking_WithParam_SharedReturnsValue()
    {
        var locker = new ReaderWriterLockSlim();

        var result = locker.ReadWriteLocking<int, string>(
            i => $"shared-{i}",
            i => $"exclusive-{i}",
            42);

        Assert.Equal("shared-42", result);
    }

    // ReadWriteLockingAsync
    [Fact]
    public async Task ReadWriteLockingAsync_SharedReturnsValue_ReturnsShared()
    {
        var locker = new ReaderWriterLockSlim();

        var result = await locker.ReadWriteLockingAsync(
            () => Task.FromResult("from-shared"),
            () => Task.FromResult("from-exclusive"));

        Assert.Equal("from-shared", result);
    }

    [Fact]
    public async Task ReadWriteLockingAsync_SharedReturnsNull_ReturnsExclusive()
    {
        var locker = new ReaderWriterLockSlim();

        var result = await locker.ReadWriteLockingAsync<string>(
            () => Task.FromResult<string>(null!),
            () => Task.FromResult("from-exclusive"));

        Assert.Equal("from-exclusive", result);
    }

    // Lock release on exception
    [Fact]
    public void ReadLocking_ExceptionInAction_ReleasesLock()
    {
        var locker = new ReaderWriterLockSlim();

        Assert.Throws<InvalidOperationException>(() =>
            locker.ReadLocking(() => throw new InvalidOperationException("test")));

        Assert.False(locker.IsReadLockHeld);
    }

    [Fact]
    public void WriteLocking_ExceptionInAction_ReleasesLock()
    {
        var locker = new ReaderWriterLockSlim();

        Assert.Throws<InvalidOperationException>(() =>
            locker.WriteLocking(() => throw new InvalidOperationException("test")));

        Assert.False(locker.IsWriteLockHeld);
    }
}
