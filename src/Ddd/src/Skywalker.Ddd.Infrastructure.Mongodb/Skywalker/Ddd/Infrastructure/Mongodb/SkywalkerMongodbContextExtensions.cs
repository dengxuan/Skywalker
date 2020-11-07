namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public static class SkywalkerMongodbContextExtensions
    {
        public static SkywalkerMongodbContext ToAbpMongoDbContext(this IMongodbContext dbContext)
        {
            var abpMongoDbContext = dbContext as SkywalkerMongodbContext;

            if (abpMongoDbContext == null)
            {
                throw new SkywalkerException($"The type {dbContext.GetType().AssemblyQualifiedName} should be convertable to {typeof(SkywalkerMongodbContext).AssemblyQualifiedName}!");
            }

            return abpMongoDbContext;
        }
    }
}