namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore
{
    public interface IDbContextProvider<TDbContext> where TDbContext : ISkywalkerDbContext
    {
        TDbContext GetDbContext();
    }
}
