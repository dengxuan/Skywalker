﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.ExceptionHandler;

namespace Skywalker.Ddd.Uow.EntityFrameworkCore;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IConnectionStringResolver _connectionStringResolver;
    private readonly ILogger<UnitOfWorkDbContextProvider<TDbContext>> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitOfWorkManager"></param>
    /// <param name="connectionStringResolver"></param>
    /// <param name="logger"></param>
    public UnitOfWorkDbContextProvider(IUnitOfWorkManager unitOfWorkManager, IConnectionStringResolver connectionStringResolver, ILogger<UnitOfWorkDbContextProvider<TDbContext>> logger)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _connectionStringResolver = connectionStringResolver;
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    /// <exception cref="SkywalkerException"></exception>
    public TDbContext GetDbContext()
    {
        var unitOfWork = _unitOfWorkManager.Current;
        if (unitOfWork == null)
        {
            throw new SkywalkerException("A DbContext can only be created inside a unit of work!");
        }

        var connectionStringName = ConnectionStringNameAttribute.GetConnectionStringName<TDbContext>();
        var connectionString = _connectionStringResolver.Resolve(connectionStringName);
        if (connectionString == null)
        {
            throw new SkywalkerException("ConnectionString Can't be null!");
        }
        var dbContextKey = $"{typeof(TDbContext).FullName}_{connectionString}";
        var databaseApi = unitOfWork.GetOrAddDatabaseApi(dbContextKey, () =>
        {
            var dbContext = CreateDbContext(unitOfWork, connectionStringName, connectionString);
            return new EfCoreDatabaseApi<TDbContext>(dbContext);
        });

        return ((EfCoreDatabaseApi<TDbContext>)databaseApi).DbContext;
    }

    private TDbContext CreateDbContext(IUnitOfWork unitOfWork, string connectionStringName, string connectionString)
    {
        var creationContext = new SkywalkerDbContextCreationContext(connectionStringName, connectionString);
        using (SkywalkerDbContextCreationContext.Use(creationContext))
        {
            var dbContext = unitOfWork.Options?.IsTransactional == true ?
                CreateDbContextWithTransaction(unitOfWork) :
                CreateDbContext(unitOfWork.ServiceProvider!);
            _logger.LogDebug("Create DbContext in unit of work: [{Id}]", unitOfWork.Id);
            if (dbContext is ISkywalkerDbContext skywalkerDbContext)
            {
                _logger.LogDebug("Initialize SkywalkerDbContext in unit of work: [{Id}]", unitOfWork.Id);
                skywalkerDbContext.Initialize(unitOfWork);
            }

            return dbContext;
        }
    }

    private static TDbContext CreateDbContext(IServiceProvider services)
    {
        return services.GetRequiredService<TDbContext>();
//#if NETSTANDARD2_0
//        return services.GetRequiredService<TDbContext>();
//#elif NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER || NETCOREAPP
//        var dbContextFactory = services.GetRequiredService<IDbContextFactory<TDbContext>>();
//        return dbContextFactory.CreateDbContext();
//#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <returns></returns>
    public TDbContext CreateDbContextWithTransaction(IUnitOfWork unitOfWork)
    {
        var transactionApiKey = $"EntityFrameworkCore_{SkywalkerDbContextCreationContext.Current.ConnectionString}";

        if (unitOfWork.FindTransactionApi(transactionApiKey) is not EfCoreTransactionApi activeTransaction)
        {
            var dbContext = CreateDbContext(unitOfWork.ServiceProvider!);

            var dbtransaction = unitOfWork.Options!.IsolationLevel.HasValue
                ? dbContext.Database.BeginTransaction(isolationLevel: unitOfWork.Options!.IsolationLevel.Value)
                : dbContext.Database.BeginTransaction();

            unitOfWork.AddTransactionApi(
                transactionApiKey,
                new EfCoreTransactionApi(
                    dbtransaction,
                    dbContext
                )
            );

            return dbContext;
        }
        else
        {
            SkywalkerDbContextCreationContext.Current.ExistingConnection = activeTransaction.DbContextTransaction.GetDbTransaction().Connection;

            var dbContext = unitOfWork.ServiceProvider!.GetRequiredService<TDbContext>();

            if (dbContext.As<DbContext>()?.HasRelationalTransactionManager() == true)
            {
                dbContext.Database.UseTransaction(activeTransaction.DbContextTransaction.GetDbTransaction());
            }
            else
            {
                dbContext.Database.BeginTransaction(); //TODO: Why not using the new created transaction?
            }

            activeTransaction.AttendedDbContexts.Add(dbContext);

            return dbContext;
        }
    }
}
