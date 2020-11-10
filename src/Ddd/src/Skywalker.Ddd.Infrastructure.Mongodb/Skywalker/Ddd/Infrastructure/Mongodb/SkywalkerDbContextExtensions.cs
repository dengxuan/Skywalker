namespace Skywalker.Ddd.Infrastructure.Mongodb
{
    public static class SkywalkerDbContextExtensions
    {
        public static SkywalkerDbContext ToAbpMongoDbContext(this ISkywalkerContext dbContext)
        {
            var abpMongoDbContext = dbContext as SkywalkerDbContext;

            if (abpMongoDbContext == null)
            {
                throw new SkywalkerException($"The type {dbContext.GetType().AssemblyQualifiedName} should be convertable to {typeof(SkywalkerDbContext).AssemblyQualifiedName}!");
            }

            return abpMongoDbContext;
        }
    }
}