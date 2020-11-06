using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;

namespace Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore
{
    public class EntityFrameworkCoreDbContextAdapter<TDbContext> : SkywalkerDatabaseAdapter<TDbContext> where TDbContext : ISkywalkerDatabase
    {
        public EntityFrameworkCoreDbContextAdapter(TDbContext dbContext) : base(dbContext)
        {
        }
    }
}
