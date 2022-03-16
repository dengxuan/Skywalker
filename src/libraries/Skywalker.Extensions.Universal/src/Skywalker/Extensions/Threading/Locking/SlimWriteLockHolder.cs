

namespace Skywalker.Extensions.Threading.Locking;

internal class SlimWriteLockHolder : ILockHolder
{
    private readonly ReaderWriterLockSlim _locker;

    public bool LockAcquired { get; private set; }

    public SlimWriteLockHolder(ReaderWriterLockSlim locker, bool waitForLock)
    {
        _locker = locker;
        if (waitForLock)
        {
            locker.EnterWriteLock();
            LockAcquired = true;
            return;
        }
        LockAcquired = locker.TryEnterWriteLock(0);
    }

    public void Dispose()
    {
        if (!LockAcquired)
        {
            return;
        }

        _locker.ExitWriteLock();
        LockAcquired = false;
    }
}
