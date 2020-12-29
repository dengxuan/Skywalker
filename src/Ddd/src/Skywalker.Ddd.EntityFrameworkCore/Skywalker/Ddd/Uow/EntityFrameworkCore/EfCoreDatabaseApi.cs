using Skywalker.Ddd.EntityFrameworkCore;
using Skywalker.Uow.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace Skywalker.Uow.EntityFrameworkCore
{
    public class EfCoreDatabaseApi<TDbContext> : IDatabaseApi, ISupportsSavingChanges
        where TDbContext : ISkywalkerDbContext
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