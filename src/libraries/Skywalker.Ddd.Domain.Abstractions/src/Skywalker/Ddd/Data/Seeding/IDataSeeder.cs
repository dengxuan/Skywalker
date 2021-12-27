using System.Threading.Tasks;

namespace Skywalker.Ddd.Data.Seeding
{
    public interface IDataSeeder
    {
        Task SeedAsync(DataSeedContext context);
    }
}