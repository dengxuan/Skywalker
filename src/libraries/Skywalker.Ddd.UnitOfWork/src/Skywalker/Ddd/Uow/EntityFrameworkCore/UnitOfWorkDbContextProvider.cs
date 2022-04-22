using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Ddd.Data;
using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.Exceptions;

namespace Skywalker.Ddd.Uow.EntityFrameworkCore;

public class UnitOfWorkDbContextProvider<TDbContext> : IDbContextProvider<TDbContext> where TDbContext : DbContext
{
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IConnectionStringResolver _connectionStringResolver;
    private readonly ILogger<UnitOfWorkDbContextProvider<TDbContext>> _logger;

    public UnitOfWorkDbContextProvider(
        IUnitOfWorkManager unitOfWorkManager,
        IConnectionStringResolver connectionStringResolver, ILogger<UnitOfWorkDbContextProvider<TDbContext>> logger)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _connectionStringResolver = connectionStringResolver;
        _logger = logger;
    }

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
        var databaseApi = unitOfWork.GetOrAddDatabaseApi(
            dbContextKey,
            () => new EfCoreDatabaseApi<TDbContext>(
                CreateDbContext(unitOfWork, connectionStringName, connectionString)
            ));

        return ((EfCoreDatabaseApi<TDbContext>)databaseApi).DbContext;
    }

    private TDbContext CreateDbContext(IUnitOfWork unitOfWork, string connectionStringName, string connectionString)
    {
        var creationContext = new SkywalkerDbContextCreationContext(connectionStringName, connectionString);
        using (SkywalkerDbContextCreationContext.Use(creationContext))
        {
            var dbContext = CreateDbContext(unitOfWork);

            if (dbContext is SkywalkerDbContext<DbContext> skywalkerDbContext)
            {
                skywalkerDbContext.Initialize(unitOfWork);
            }

            return dbContext;
        }
    }

    private TDbContext CreateDbContext(IUnitOfWork unitOfWork)
    {
        return unitOfWork.Options!.IsTransactional
            ? CreateDbContextWithTransaction(unitOfWork)
            : unitOfWork.ServiceProvider!.GetRequiredService<TDbContext>();
    }

    public TDbContext CreateDbContextWithTransaction(IUnitOfWork unitOfWork)
    {
        var transactionApiKey = $"EntityFrameworkCore_{SkywalkerDbContextCreationContext.Current.ConnectionString}";

        if (unitOfWork.FindTransactionApi(transactionApiKey) is not EfCoreTransactionApi activeTransaction)
        {
            var dbContext = unitOfWork.ServiceProvider!.GetRequiredService<TDbContext>();

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

            if (dbContext.As<DbContext>().HasRelationalTransactionManager())
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
