using System.Threading.Tasks;

namespace Skywalker.Ddd.Data.Seeding;

public interface IDataSeedContributor
{
    Task SeedAsync(DataSeedContext context);
}
