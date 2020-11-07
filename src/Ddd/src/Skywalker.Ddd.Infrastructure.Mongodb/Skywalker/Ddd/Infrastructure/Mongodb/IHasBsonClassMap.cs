using MongoDB.Bson.Serialization;

namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface IHasBsonClassMap
    {
        BsonClassMap GetMap();
    }
}