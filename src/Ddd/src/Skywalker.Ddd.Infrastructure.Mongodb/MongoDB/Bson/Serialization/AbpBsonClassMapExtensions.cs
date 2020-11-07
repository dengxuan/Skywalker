namespace MongoDB.Bson.Serialization
{
    public static class AbpBsonClassMapExtensions
    {
        public static void ConfigureAbpConventions(this BsonClassMap map)
        {
            map.AutoMap();
        }
    }
}
