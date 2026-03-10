namespace Skywalker.Extensions.Threading.Locking;

internal class SlimUpgradeableReadLockHolder : IUpgradeableLockHolder
{
    private readonly ReaderWriterLockSlim _locker;
    private readonly bool _wasLockAlreadyHeld;
    private SlimWriteLockHolder? _writerLock;

    public bool LockAcquired { get; private set; }

    public SlimUpgradeableReadLockHolder(ReaderWriterLockSlim locker, bool waitForLock, bool wasLockAlreadyHelf)
    {
        _locker = locker;
        if (wasLockAlreadyHelf)
        {
            LockAcquired = true;
            _wasLockAlreadyHeld = true;
            return;
        }

        if (waitForLock)
        {
            locker.EnterUpgradeableReadLock();
            LockAcquired = true;
            return;
        }

        LockAcquired = locker.TryEnterUpgradeableReadLock(0);
    }

    public void Dispose()
    {
        if (_writerLock != null && _writerLock.LockAcquired)
        {
            _writerLock.Dispose();
            _writerLock = null;
        }
        if (!LockAcquired) return;
        if (!_wasLockAlreadyHeld)
        {
            _locker.ExitUpgradeableReadLock();
        }
        LockAcquired = false;

    }

    public ILockHolder Upgrade()
    {
        return Upgrade(true);
    }

    public ILockHolder Upgrade(bool waitForLock)
    {
        if (_locker.IsWriteLockHeld)
        {
            return NoOpLock.Lock;
        }

        _writerLock = new SlimWriteLockHolder(_locker, waitForLock);
        return _writerLock;
    }
}
