namespace Skywalker.Ddd.Data.Seeding;

public static class DataSeederExtensions
{
    public static Task SeedAsync(this IDataSeeder seeder)
    {
        return seeder.SeedAsync(new DataSeedContext());
    }
}
