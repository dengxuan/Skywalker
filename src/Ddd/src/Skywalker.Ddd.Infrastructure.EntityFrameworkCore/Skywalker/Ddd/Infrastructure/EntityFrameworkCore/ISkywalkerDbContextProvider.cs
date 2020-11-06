using Microsoft.EntityFrameworkCore;

namespace Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore
{
    public interface ISkywalkerDbContextProvider<TDbContext> where TDbContext : DbContext
    {
        TDbContext GetDbContext();
    }
}
