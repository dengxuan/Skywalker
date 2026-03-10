// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow;

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IUnitOfWork : IDatabaseApiContainer, ITransactionApiContainer, IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// 
    /// </summary>
    Dictionary<string, object> Items { get; }

    /// <summary>
    /// TODO: Switch to OnFailed (sync) and OnDisposed (sync) methods to be compatible with OnCompleted
    /// </summary>
    event EventHandler<UnitOfWorkFailedEventArgs>? Failed;

    /// <summary>
    /// 
    /// </summary>

    event EventHandler<UnitOfWorkEventArgs>? Disposed;

    /// <summary>
    /// 
    /// </summary>
    IUnitOfWorkOptions? Options { get; }

    /// <summary>
    /// 
    /// </summary>
    IUnitOfWork? Outer { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsReserved { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// 
    /// </summary>
    string? ReservationName { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outer"></param>
    void SetOuter(IUnitOfWork? outer);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    void Initialize(UnitOfWorkOptions options);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    void Reserve(string reservationName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CompleteAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handler"></param>
    void OnCompleted(Func<Task> handler);
}
