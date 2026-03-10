namespace Skywalker.Extensions.Threading.Locking;

internal class NoOpLock : ILockHolder
{
    public static readonly ILockHolder Lock = new NoOpLock();

    public void Dispose()
    {

    }

    public bool LockAcquired
    {
        get { return true; }
    }
}
