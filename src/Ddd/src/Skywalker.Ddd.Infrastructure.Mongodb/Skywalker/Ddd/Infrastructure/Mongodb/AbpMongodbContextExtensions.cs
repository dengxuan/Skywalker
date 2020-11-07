using Skywalker;

namespace Volo.Abp.MongoDB
{
    public static class AbpMongoDbContextExtensions
    {
        public static AbpMongoDbContext ToAbpMongoDbContext(this IMongodbContext dbContext)
        {
            var abpMongoDbContext = dbContext as AbpMongoDbContext;

            if (abpMongoDbContext == null)
            {
                throw new SkywalkerException($"The type {dbContext.GetType().AssemblyQualifiedName} should be convertable to {typeof(AbpMongoDbContext).AssemblyQualifiedName}!");
            }

            return abpMongoDbContext;
        }
    }
}