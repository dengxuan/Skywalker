namespace Skywalker.Extensions.Threading.Locking;

internal class SlimReadWriteLock : Lock
{
    private readonly ReaderWriterLockSlim _locker = new(LockRecursionPolicy.NoRecursion);

    public override IUpgradeableLockHolder ForReadingUpgradeable()
    {
        return ForReadingUpgradeable(true);
    }

    public override ILockHolder ForReading()
    {
        return ForReading(true);
    }

    public override ILockHolder ForWriting()
    {
        return ForWriting(true);
    }

    public override IUpgradeableLockHolder ForReadingUpgradeable(bool waitForLock)
    {
        return new SlimUpgradeableReadLockHolder(_locker, waitForLock, _locker.IsUpgradeableReadLockHeld || _locker.IsWriteLockHeld);
    }

    public override ILockHolder ForReading(bool waitForLock)
    {
        if (_locker.IsReadLockHeld || _locker.IsUpgradeableReadLockHeld || _locker.IsWriteLockHeld)
        {
            return NoOpLock.Lock;
        }

        return new SlimReadLockHolder(_locker, waitForLock);
    }

    public override ILockHolder ForWriting(bool waitForLock)
    {
        if (_locker.IsWriteLockHeld)
        {
            return NoOpLock.Lock;
        }

        return new SlimWriteLockHolder(_locker, waitForLock);
    }

    public bool IsReadLockHeld
    {
        get { return _locker.IsReadLockHeld; }
    }

    public bool IsUpgradeableReadLockHeld
    {
        get { return _locker.IsUpgradeableReadLockHeld; }
    }

    public bool IsWriteLockHeld
    {
        get { return _locker.IsWriteLockHeld; }
    }
}
