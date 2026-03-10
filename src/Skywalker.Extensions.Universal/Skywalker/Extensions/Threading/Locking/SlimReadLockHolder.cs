
using System.Threading;

namespace Skywalker.Extensions.Threading.Locking;
internal class SlimReadLockHolder : ILockHolder
{
    private readonly ReaderWriterLockSlim _locker;

    public bool LockAcquired { get; private set; }

    public SlimReadLockHolder(ReaderWriterLockSlim locker, bool waitForLock)
    {
        _locker = locker;
        if (waitForLock)
        {
            locker.EnterReadLock();
            LockAcquired = true;
            return;
        }
        LockAcquired = locker.TryEnterReadLock(0);
    }

    public void Dispose()
    {
        if (!LockAcquired)
        {
            return;
        }

        _locker.ExitReadLock();
        LockAcquired = false;
    }
}
