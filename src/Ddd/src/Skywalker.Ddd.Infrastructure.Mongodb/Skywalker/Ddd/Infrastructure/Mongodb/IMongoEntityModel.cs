using System;

namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public interface IMongoEntityModel
    {
        Type EntityType { get; }

        string CollectionName { get; }
    }
}