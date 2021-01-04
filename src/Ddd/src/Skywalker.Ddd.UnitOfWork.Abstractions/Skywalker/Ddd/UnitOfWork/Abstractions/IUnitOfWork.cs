using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface IUnitOfWork : IDatabaseApiContainer, ITransactionApiContainer, IDisposable
    {
        Guid Id { get; }

        Dictionary<string, object> Items { get; }

        //TODO: Switch to OnFailed (sync) and OnDisposed (sync) methods to be compatible with OnCompleted
        event EventHandler<UnitOfWorkFailedEventArgs> Failed;

        event EventHandler<UnitOfWorkEventArgs> Disposed;

        IAbpUnitOfWorkOptions? Options { get; }

        IUnitOfWork? Outer { get; }

        bool IsReserved { get; }

        bool IsDisposed { get; }

        bool IsCompleted { get; }

        string? ReservationName { get; }

        void SetOuter([MaybeNull] IUnitOfWork? outer);

        void Initialize([NotNull] AbpUnitOfWorkOptions options);

        void Reserve([NotNull] string reservationName);

        Task SaveChangesAsync(CancellationToken cancellationToken = default);

        Task CompleteAsync(CancellationToken cancellationToken = default);

        Task RollbackAsync(CancellationToken cancellationToken = default);

        void OnCompleted(Func<Task> handler);
    }
}
