namespace Skywalker.Extensions.Threading.Locking;

internal class NoOpUpgradeableLock : IUpgradeableLockHolder
{
    public static readonly IUpgradeableLockHolder Lock = new NoOpUpgradeableLock();

    public void Dispose()
    {

    }

    public bool LockAcquired
    {
        get { return true; }
    }

    public ILockHolder Upgrade()
    {
        return NoOpLock.Lock;
    }

    public ILockHolder Upgrade(bool waitForLock)
    {
        return NoOpLock.Lock;
    }
}
