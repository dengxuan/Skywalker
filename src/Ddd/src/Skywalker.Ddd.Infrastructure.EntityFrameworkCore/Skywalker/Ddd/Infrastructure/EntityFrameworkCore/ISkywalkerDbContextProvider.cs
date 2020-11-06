using Microsoft.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore;

namespace Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore
{
    public interface ISkywalkerDbContextProvider<TDbContext> where TDbContext : SkywalkerDbContext<TDbContext>
    {
        TDbContext GetDbContext();
    }
}
