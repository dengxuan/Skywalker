namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface IMongoModelSource
    {
        MongoDbContextModel GetModel(SkywalkerDbContext dbContext);
    }
}