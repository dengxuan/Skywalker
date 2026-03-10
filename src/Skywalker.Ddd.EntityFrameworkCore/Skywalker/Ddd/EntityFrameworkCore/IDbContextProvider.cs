using Microsoft.EntityFrameworkCore;

namespace Skywalker.Ddd.EntityFrameworkCore;

public interface IDbContextProvider<TDbContext> where TDbContext : DbContext
{
    TDbContext GetDbContext();
}
