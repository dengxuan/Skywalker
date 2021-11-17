using System;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Extensions.Threading
{
    /// <summary>
    /// Lock helper to make locking easier.
    /// </summary>
    public static class ReaderWriterLockSlimExtensions
    {
        #region ReadLocking

        public static void ReadLocking(this ReaderWriterLockSlim locker, Action action)
        {
            locker.ReadLocking<object>((@object) => action?.Invoke(), default);
        }

        public static void ReadLocking<T>(this ReaderWriterLockSlim locker, Action<T?> action, T? @object)
        {
            try
            {
                locker.EnterReadLock();
                action?.Invoke(@object);
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static async Task ReadLockingAsync(this ReaderWriterLockSlim locker, Action action)
        {
            await locker.ReadLockingAsync<object>((@object) => action?.Invoke(), default);
        }

        public static async Task ReadLockingAsync<T>(this ReaderWriterLockSlim locker, Action<T?> action, T? @object)
        {
            await Task.Run(() => locker.ReadLocking(action, @object));
        }

        public static TResult ReadLocking<TResult>(this ReaderWriterLockSlim locker, Func<TResult> method)
        {
            return locker.ReadLocking<object, TResult>((@object) => method.Invoke(), default);
        }

        public static TResult ReadLocking<T, TResult>(this ReaderWriterLockSlim locker, Func<T?, TResult> method, T? @object)
        {
            try
            {
                locker.EnterReadLock();
                return method.Invoke(@object);
            }
            finally
            {
                locker.ExitReadLock();
            }
        }

        public static async Task<TResult> ReadLockingAsync<TResult>(this ReaderWriterLockSlim locker, Func<TResult> method)
        {
            return await locker.ReadLockingAsync<object, TResult>((@object) => method(), default);
        }

        public static async Task<TResult> ReadLockingAsync<T, TResult>(this ReaderWriterLockSlim locker, Func<T?, TResult> method, T? @object)
        {
            return await Task.Run(() => locker.ReadLocking(method, @object));
        }

        #endregion

        #region WriteLocking

        public static void WriteLocking(this ReaderWriterLockSlim locker, Action action)
        {
            locker.WriteLocking<object>((@object) => action?.Invoke(), default);
        }

        public static void WriteLocking<T>(this ReaderWriterLockSlim locker, Action<T?> action, T? @object)
        {
            try
            {
                locker.EnterWriteLock();
                action?.Invoke(@object);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public static async Task WriteLockingAsync(this ReaderWriterLockSlim locker, Action action)
        {
            await locker.WriteLockingAsync<object>((@object) => action?.Invoke(), default);
        }

        public static async Task WriteLockingAsync<T>(this ReaderWriterLockSlim locker, Action<T?> action, T? @object)
        {
            await Task.Run(() => locker.WriteLocking(action, @object));
        }

        public static TResult WriteLocking<TResult>(this ReaderWriterLockSlim locker, Func<TResult> method)
        {
            return locker.WriteLocking<object, TResult>((@object) => method(), default);
        }

        public static TResult WriteLocking<T, TResult>(this ReaderWriterLockSlim locker, Func<T?, TResult> method, T? @object)
        {
            try
            {
                locker.EnterWriteLock();
                return method(@object);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }

        public static async Task<TResult> WriteLockingAsync<TResult>(this ReaderWriterLockSlim locker, Func<TResult> method)
        {
            return await locker.WriteLockingAsync<object, TResult>((@object) => method(), null);
        }

        public static async Task<TResult> WriteLockingAsync<T, TResult>(this ReaderWriterLockSlim locker, Func<T?, TResult> method, T? @object)
        {
            return await Task.Run(() => locker.WriteLocking(method, @object));
        }

        #endregion

        #region ReadWriteLocking

        public static TResult ReadWriteLocking<TResult>(this ReaderWriterLockSlim locker, Func<TResult> shard, Func<TResult> once)
        {
            return locker.ReadWriteLocking<object, TResult>((@object) => { return shard(); }, (@object) => { return once(); }, default);
        }

        public static TResult ReadWriteLocking<T, TResult>(this ReaderWriterLockSlim locker, Func<T?, TResult> shard, Func<T?, TResult> once, T? @object)
        {
            try
            {
                locker.EnterReadLock();
                var result = shard(@object);
                if (result != null)
                {
                    return result;
                }
            }
            finally
            {
                locker.ExitReadLock();
            }

            try
            {
                locker.EnterUpgradeableReadLock();
                var result = shard(@object);
                if (result == null)
                {
                    try
                    {
                        locker.TryEnterWriteLock(3000);
                        result = once(@object);
                    }
                    finally
                    {
                        locker.ExitWriteLock();
                    }
                }
                return result;
            }
            finally
            {
                locker.ExitUpgradeableReadLock();
            }
        }

        public static async Task<TResult> ReadWriteLockingAsync<TResult>(this ReaderWriterLockSlim locker, Func<TResult> shard, Func<TResult> once)
        {
            return await locker.ReadWriteLockingAsync<object, TResult>((@object) => shard(), (@object) => once(), default);
        }

        public static async Task<TResult> ReadWriteLockingAsync<T, TResult>(this ReaderWriterLockSlim locker, Func<T?, TResult> shard, Func<T?, TResult> once, T? @object)
        {
            return await Task.Run(() => locker.ReadWriteLocking(shard, once, @object));
        }

        #endregion
    }
}
