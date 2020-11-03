using System.Threading.Tasks;

namespace Skywalker.Data.Seeding
{
    public interface IDataSeedContributor
    {
        Task SeedAsync(DataSeedContext context);
    }
}