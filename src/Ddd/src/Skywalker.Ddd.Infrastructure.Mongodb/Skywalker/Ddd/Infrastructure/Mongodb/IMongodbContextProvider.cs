namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface IMongoDbContextProvider<out TMongoDbContext> where TMongoDbContext : IMongodbContext
    {
        TMongoDbContext GetDbContext();
    }
}