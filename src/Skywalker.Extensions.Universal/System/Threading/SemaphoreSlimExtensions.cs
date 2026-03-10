using Skywalker;

namespace System.Threading;

public static class SemaphoreSlimExtensions
{
    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim)
    {
        await semaphoreSlim.WaitAsync();
        return semaphoreSlim.GetDispose();
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(cancellationToken);
        return semaphoreSlim.GetDispose();
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
    {
        await semaphoreSlim.WaitAsync(millisecondsTimeout);
        return semaphoreSlim.GetDispose();
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(millisecondsTimeout, cancellationToken);
        return semaphoreSlim.GetDispose();
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
    {
        await semaphoreSlim.WaitAsync(timeout);
        return semaphoreSlim.GetDispose();
    }

    public static async Task<IDisposable> LockAsync(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken)
    {
        await semaphoreSlim.WaitAsync(timeout, cancellationToken);
        return semaphoreSlim.GetDispose();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim)
    {
        semaphoreSlim.Wait();
        return semaphoreSlim.GetDispose();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, CancellationToken cancellationToken)
    {
        semaphoreSlim.Wait(cancellationToken);
        return semaphoreSlim.GetDispose();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout)
    {
        semaphoreSlim.Wait(millisecondsTimeout);
        return semaphoreSlim.GetDispose();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, int millisecondsTimeout, CancellationToken cancellationToken)
    {
        semaphoreSlim.Wait(millisecondsTimeout, cancellationToken);
        return semaphoreSlim.GetDispose();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout)
    {
        semaphoreSlim.Wait(timeout);
        return semaphoreSlim.GetDispose();
    }

    public static IDisposable Lock(this SemaphoreSlim semaphoreSlim, TimeSpan timeout, CancellationToken cancellationToken)
    {
        semaphoreSlim.Wait(timeout, cancellationToken);
        return semaphoreSlim.GetDispose();
    }

    private static IDisposable GetDispose(this SemaphoreSlim semaphoreSlim)
    {
        return new DisposeAction(() =>
        {
            semaphoreSlim.Release();
        });
    }
}
