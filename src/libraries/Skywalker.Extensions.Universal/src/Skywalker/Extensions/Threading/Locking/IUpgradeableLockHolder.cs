namespace Skywalker.Extensions.Threading.Locking;

public interface IUpgradeableLockHolder : ILockHolder
{
    ILockHolder Upgrade();

    ILockHolder Upgrade(bool waitForLock);
}
