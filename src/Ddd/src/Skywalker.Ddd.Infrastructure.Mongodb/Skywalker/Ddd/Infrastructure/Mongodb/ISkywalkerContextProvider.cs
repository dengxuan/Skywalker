namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface ISkywalkerContextProvider<out TDbContext> where TDbContext : ISkywalkerContext
    {
        TDbContext GetDbContext();
    }
}