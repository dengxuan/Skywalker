using Skywalker.EntityFrameworkCore;

namespace Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore
{
    public interface IEntityFrameworkCoreDbContextProvider<TDbContext> where TDbContext : IEntityFrameworkCoreDbContext
    {
        TDbContext GetDbContext();
    }
}
