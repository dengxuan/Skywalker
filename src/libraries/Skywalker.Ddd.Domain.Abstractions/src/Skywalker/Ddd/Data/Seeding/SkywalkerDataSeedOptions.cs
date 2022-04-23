namespace Skywalker.Ddd.Data.Seeding;

public class SkywalkerDataSeedOptions
{
    public DataSeedContributorList Contributors { get; }

    public SkywalkerDataSeedOptions()
    {
        Contributors = new DataSeedContributorList();
    }
}
