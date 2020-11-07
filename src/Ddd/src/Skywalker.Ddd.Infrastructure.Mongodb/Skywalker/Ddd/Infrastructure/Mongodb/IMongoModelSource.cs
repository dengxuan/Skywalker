namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface IMongoModelSource
    {
        MongoDbContextModel GetModel(SkywalkerMongodbContext dbContext);
    }
}