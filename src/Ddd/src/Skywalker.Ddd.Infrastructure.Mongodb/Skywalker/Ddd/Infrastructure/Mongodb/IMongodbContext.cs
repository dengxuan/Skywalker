using MongoDB.Driver;

namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface IMongodbContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<T> Collection<T>();
    }
}