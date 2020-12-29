using Skywalker.EntityFrameworkCore;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration
{
    public interface ISkywalkerDbContextConfigurer
    {
        void Configure(SkywalkerDbContextConfigurationContext context);
    }

    public interface ISkywalkerDbContextConfigurer<TDbContext> where TDbContext :  SkywalkerDbContext<TDbContext>
    {
        void Configure(SkywalkerDbContextConfigurationContext<TDbContext> context);
    }
}