using MongoDB.Bson.Serialization;
using System;
using System.Reflection;

namespace Volo.Abp.MongoDB
{
    public static class AbpBsonClassMapExtensions
    {
        public static void ConfigureAbpConventions(this BsonClassMap map)
        {
            map.AutoMap();
        }
    }
}
