using Microsoft.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore;

namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore
{
    public interface ISkywalkerDbContextProvider<TDbContext> where TDbContext : SkywalkerDbContext<TDbContext>
    {
        TDbContext GetDbContext();
    }
}
