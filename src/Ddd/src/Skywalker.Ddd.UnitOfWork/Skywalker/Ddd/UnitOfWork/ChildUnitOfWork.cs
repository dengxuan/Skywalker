using Skywalker.Ddd.UnitOfWork.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork
{
    internal class ChildUnitOfWork : IUnitOfWork
    {
        public Guid Id => _parent.Id;

        public IAbpUnitOfWorkOptions? Options => _parent.Options;

        public IUnitOfWork? Outer => _parent.Outer;

        public bool IsReserved => _parent.IsReserved;

        public bool IsDisposed => _parent.IsDisposed;

        public bool IsCompleted => _parent.IsCompleted;

        public string? ReservationName => _parent.ReservationName;

        public event EventHandler<UnitOfWorkFailedEventArgs>? Failed;
        public event EventHandler<UnitOfWorkEventArgs>? Disposed;

        public IServiceProvider ServiceProvider => _parent.ServiceProvider!;

        public Dictionary<string, object> Items => _parent.Items;

        private readonly IUnitOfWork _parent;

        public ChildUnitOfWork([NotNull] IUnitOfWork parent)
        {
            Check.NotNull(parent, nameof(parent));

            _parent = parent;

            _parent.Failed += (sender, args) => { Failed?.InvokeSafely(sender!, args); };
            _parent.Disposed += (sender, args) => { Disposed?.InvokeSafely(sender!, args); };
        }

        public void SetOuter([MaybeNull]IUnitOfWork? outer)
        {
            _parent.SetOuter(outer);
        }

        public void Initialize([NotNull]AbpUnitOfWorkOptions options)
        {
            _parent.Initialize(options);
        }

        public void Reserve([NotNull]string reservationName)
        {
            _parent.Reserve(reservationName);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _parent.SaveChangesAsync(cancellationToken);
        }

        public Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _parent.RollbackAsync(cancellationToken);
        }

        public void OnCompleted(Func<Task> handler)
        {
            _parent.OnCompleted(handler);
        }

        public IDatabaseApi? FindDatabaseApi([NotNull]string key)
        {
            return _parent.FindDatabaseApi(key);
        }

        public void AddDatabaseApi([NotNull]string key, [NotNull]IDatabaseApi api)
        {
            _parent.AddDatabaseApi(key, api);
        }

        public IDatabaseApi GetOrAddDatabaseApi([NotNull] string key, [NotNull] Func<IDatabaseApi> factory)
        {
            return _parent.GetOrAddDatabaseApi(key, factory);
        }

        public ITransactionApi? FindTransactionApi([NotNull] string key)
        {
            return _parent.FindTransactionApi(key);
        }

        public void AddTransactionApi([NotNull] string key, [NotNull] ITransactionApi api)
        {
            _parent.AddTransactionApi(key, api);
        }

        public ITransactionApi GetOrAddTransactionApi([NotNull] string key, [NotNull] Func<ITransactionApi> factory)
        {
            return _parent.GetOrAddTransactionApi(key, factory);
        }

        public void Dispose()
        {

        }

        public override string ToString()
        {
            return $"[UnitOfWork {Id}]";
        }
    }
}