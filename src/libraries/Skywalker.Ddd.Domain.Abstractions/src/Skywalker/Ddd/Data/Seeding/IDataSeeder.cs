using System.Threading.Tasks;

namespace Skywalker.Data.Seeding
{
    public interface IDataSeeder
    {
        Task SeedAsync(DataSeedContext context);
    }
}