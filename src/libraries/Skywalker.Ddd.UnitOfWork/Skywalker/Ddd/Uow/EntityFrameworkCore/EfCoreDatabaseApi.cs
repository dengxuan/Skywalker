using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow.EntityFrameworkCore;

public class EfCoreDatabaseApi<TDbContext> : IDatabaseApi, ISupportsSavingChanges where TDbContext : DbContext
{
    public TDbContext DbContext { get; }

    public EfCoreDatabaseApi(TDbContext dbContext)
    {
        DbContext = dbContext;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.SaveChangesAsync(cancellationToken);
    }
}
