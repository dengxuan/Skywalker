using MongoDB.Driver;

namespace Volo.Abp.MongoDB
{
    public interface IMongodbContext
    {
        IMongoDatabase Database { get; }

        IMongoCollection<T> Collection<T>();
    }
}