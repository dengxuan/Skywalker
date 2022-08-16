using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow.EntityFrameworkCore;

public class EfCoreTransactionApi : ITransactionApi, ISupportsRollback
{
    public IDbContextTransaction DbContextTransaction { get; }
    public DbContext StarterDbContext { get; }
    public List<DbContext> AttendedDbContexts { get; }

    public EfCoreTransactionApi(IDbContextTransaction dbContextTransaction, DbContext starterDbContext)
    {
        DbContextTransaction = dbContextTransaction;
        StarterDbContext = starterDbContext;
        AttendedDbContexts = new List<DbContext>();
    }

    public Task CommitAsync()
    {
        Commit();
        return Task.CompletedTask;
    }

    protected void Commit()
    {
        DbContextTransaction.Commit();

        foreach (var dbContext in AttendedDbContexts)
        {
            if (dbContext.As<DbContext>()?.HasRelationalTransactionManager() == true)
            {
                continue; //Relational databases use the shared transaction
            }

            dbContext.Database.CommitTransaction();
        }
    }

    public void Dispose()
    {
        DbContextTransaction.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Rollback()
    {
        DbContextTransaction.Rollback();
    }

    public Task RollbackAsync(CancellationToken cancellationToken)
    {
        DbContextTransaction.Rollback();
        return Task.CompletedTask;
    }
}
