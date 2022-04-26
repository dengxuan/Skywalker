using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Data.Seeding;

public interface IDataSeedContributor : ITransientDependency
{
    Task SeedAsync(DataSeedContext context);
}
