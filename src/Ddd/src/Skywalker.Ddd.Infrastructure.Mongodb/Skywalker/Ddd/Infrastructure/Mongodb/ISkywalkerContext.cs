using MongoDB.Driver;

namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface ISkywalkerContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<T> Collection<T>();
    }
}