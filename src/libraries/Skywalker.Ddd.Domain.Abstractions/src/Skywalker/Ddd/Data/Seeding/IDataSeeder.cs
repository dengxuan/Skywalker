using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Data.Seeding;

public interface IDataSeeder : ISingletonDependency
{
    Task SeedAsync(DataSeedContext context);
}
