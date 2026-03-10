namespace Skywalker.Extensions.Threading.Locking;

public abstract class Lock
{
    public abstract IUpgradeableLockHolder ForReadingUpgradeable();
    public abstract ILockHolder ForReading();
    public abstract ILockHolder ForWriting();

    public abstract IUpgradeableLockHolder ForReadingUpgradeable(bool waitForLock);
    public abstract ILockHolder ForReading(bool waitForLock);
    public abstract ILockHolder ForWriting(bool waitForLock);

    /// <summary>
    /// Creates a new lock.
    /// </summary>
    /// <returns></returns>
    public static Lock Create()
    {
        return new SlimReadWriteLock();
    }
}
