namespace Skywalker.Extensions.Threading.Locking;

public interface ILockHolder : IDisposable
{
    bool LockAcquired { get; }
}
