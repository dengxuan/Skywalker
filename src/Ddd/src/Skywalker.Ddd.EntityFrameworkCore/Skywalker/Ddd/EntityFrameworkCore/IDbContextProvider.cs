namespace Skywalker.Ddd.EntityFrameworkCore
{
    public interface IDbContextProvider<TDbContext> where TDbContext : ISkywalkerDbContext
    {
        TDbContext GetDbContext();
    }
}
