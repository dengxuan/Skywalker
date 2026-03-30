// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Skywalker.Exceptions;
using Skywalker.Ddd.Uow.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// 工作单元实现
/// </summary>
[ExposeServices(typeof(IUnitOfWork), typeof(IDatabaseApiContainer), typeof(ITransactionApiContainer))]
public class UnitOfWork : IUnitOfWork, ITransientDependency
{
    /// <summary>
    /// 
    /// </summary>
    public const string UnitOfWorkReservationName = "_SkywalkerActionUnitOfWork";

    /// <summary>
    /// 
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// 
    /// </summary>
    public IUnitOfWorkOptions? Options { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public IUnitOfWork? Outer { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsReserved { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsCompleted { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public string? ReservationName { get; set; }

    /// <summary>
    /// 
    /// </summary>
    protected List<Func<Task>> CompletedHandlers { get; } = new List<Func<Task>>();

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<UnitOfWorkFailedEventArgs>? Failed;

    /// <summary>
    /// 
    /// </summary>
    public event EventHandler<UnitOfWorkEventArgs>? Disposed;

    /// <summary>
    /// 
    /// </summary>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 
    /// </summary>
    public Dictionary<string, object> Items { get; }

    private readonly Dictionary<string, IDatabaseApi> _databaseApis;
    private readonly Dictionary<string, ITransactionApi> _transactionApis;
    private readonly UnitOfWorkDefaultOptions _defaultOptions;
    private readonly ILogger<UnitOfWork> _logger;

    private Exception? _exception;
    private bool _isCompleting;
    private bool _isRolledback;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public UnitOfWork(IServiceProvider serviceProvider, IOptions<UnitOfWorkDefaultOptions> options, ILogger<UnitOfWork> logger)
    {
        ServiceProvider = serviceProvider;
        _defaultOptions = options.Value;
        _logger = logger;

        _databaseApis = new Dictionary<string, IDatabaseApi>();
        _transactionApis = new Dictionary<string, ITransactionApi>();

        Items = new Dictionary<string, object>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <exception cref="SkywalkerException"></exception>
    public virtual void Initialize(UnitOfWorkOptions options)
    {
        options.NotNull(nameof(options));

        if (Options != null)
        {
            throw new SkywalkerException("This unit of work is already initialized before!");
        }

        Options = _defaultOptions.Normalize(options.Clone());
        IsReserved = false;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    public virtual void Reserve(string reservationName)
    {
        reservationName.NotNull(nameof(reservationName));

        ReservationName = reservationName;
        IsReserved = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outer"></param>
    public virtual void SetOuter(IUnitOfWork? outer)
    {
        Outer = outer;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsSavingChanges supportsSavingChanges)
            {
                await supportsSavingChanges.SaveChangesAsync(cancellationToken);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<IDatabaseApi> GetAllActiveDatabaseApis()
    {
        return _databaseApis.Values.ToImmutableList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<ITransactionApi> GetAllActiveTransactionApis()
    {
        return _transactionApis.Values.ToImmutableList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledback)
        {
            return;
        }

        PreventMultipleComplete();

        try
        {
            _isCompleting = true;
            await SaveChangesAsync(cancellationToken);
            await CommitTransactionsAsync();
            IsCompleted = true;
            await OnCompletedAsync();
        }
        catch (Exception ex)
        {
            _exception = ex;
            throw;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public virtual async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_isRolledback)
        {
            return;
        }

        _isRolledback = true;

        await RollbackAllAsync(cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public IDatabaseApi? FindDatabaseApi(string key)
    {
        return _databaseApis.GetOrDefault(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="api"></param>
    /// <exception cref="SkywalkerException"></exception>
    public void AddDatabaseApi(string key, IDatabaseApi api)
    {
        key.NotNull(nameof(key));
        api.NotNull(nameof(api));

        if (_databaseApis.ContainsKey(key))
        {
            throw new SkywalkerException("There is already a database API in this unit of work with given key: " + key);
        }

        _databaseApis.Add(key, api);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory)
    {
        key.NotNull(nameof(key));
        factory.NotNull(nameof(factory));

        return _databaseApis.GetOrAdd(key, factory);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public ITransactionApi? FindTransactionApi(string key)
    {
        key.NotNull(nameof(key));

        return _transactionApis.GetOrDefault(key);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="api"></param>
    /// <exception cref="SkywalkerException"></exception>
    public void AddTransactionApi(string key, ITransactionApi api)
    {
        key.NotNull(nameof(key));
        api.NotNull(nameof(api));

        if (_transactionApis.ContainsKey(key))
        {
            throw new SkywalkerException("There is already a transaction API in this unit of work with given key: " + key);
        }

        _transactionApis.Add(key, api);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory)
    {
        key.NotNull(nameof(key));
        factory.NotNull(nameof(factory));

        return _transactionApis.GetOrAdd(key, factory);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handler"></param>
    public void OnCompleted(Func<Task> handler)
    {
        CompletedHandlers.Add(handler);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual async Task OnCompletedAsync()
    {
        foreach (var handler in CompletedHandlers)
        {
            await handler.Invoke();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnFailed()
    {
        Failed?.InvokeSafely(this, new UnitOfWorkFailedEventArgs(this, _exception, _isRolledback));
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void OnDisposed()
    {
        Disposed?.InvokeSafely(this, new UnitOfWorkEventArgs(this));
    }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Dispose()
    {
        if (IsDisposed)
        {
            return;
        }

        IsDisposed = true;

        DisposeTransactions();

        if (!IsCompleted || _exception != null)
        {
            OnFailed();
        }

        OnDisposed();

        GC.SuppressFinalize(this);
    }

    private void DisposeTransactions()
    {
        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            try
            {
                transactionApi.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction: {Id} Dispose Failed!", Id);
            }
        }
    }

    private void PreventMultipleComplete()
    {
        if (IsCompleted || _isCompleting)
        {
            throw new SkywalkerException("Complete is called before!");
        }
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void RollbackAll()
    {
        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            try
            {
                (databaseApi as ISupportsRollback)?.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction: {Id} Rollback Failed!", Id);
            }
        }

        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            try
            {
                (transactionApi as ISupportsRollback)?.Rollback();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Transaction: {Id} Rollback Failed!", Id);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    protected virtual async Task RollbackAllAsync(CancellationToken cancellationToken)
    {
        foreach (var databaseApi in GetAllActiveDatabaseApis())
        {
            if (databaseApi is ISupportsRollback supportsRollback)
            {
                try
                {
                    await supportsRollback.RollbackAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Transaction: {Id} Rollback Failed!", Id);
                }
            }
        }

        foreach (var transactionApi in GetAllActiveTransactionApis())
        {
            if (transactionApi is ISupportsRollback supportsRollback)
            {
                try
                {
                    await supportsRollback.RollbackAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Transaction: {Id} Rollback Failed!", Id);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual async Task CommitTransactionsAsync()
    {
        foreach (var transaction in GetAllActiveTransactionApis())
        {
            await transaction.CommitAsync();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"[UnitOfWork {Id}]";
    }
}
