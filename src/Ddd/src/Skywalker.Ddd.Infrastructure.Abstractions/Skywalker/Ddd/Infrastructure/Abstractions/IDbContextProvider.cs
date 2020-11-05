namespace Skywalker.Ddd.Infrastructure.Abstractions
{
    public interface IDbContextProvider<out TDbContext> where TDbContext : IDbContext
    {
        TDbContext GetDbContext();
    }
}