using Microsoft.EntityFrameworkCore;
using Skywalker.Ddd.UnitOfWork.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.UnitOfWork.EntityFrameworkCore
{
    public class EfCoreDatabaseApi<TDbContext> : IDatabaseApi, ISupportsSavingChanges
        where TDbContext : DbContext
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
}