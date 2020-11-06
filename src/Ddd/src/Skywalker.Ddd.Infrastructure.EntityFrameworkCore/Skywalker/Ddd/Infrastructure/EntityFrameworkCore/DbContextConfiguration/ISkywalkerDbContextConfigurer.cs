using Skywalker.EntityFrameworkCore;

namespace Skywalker.Ddd.Infrastructure.DbContextConfiguration
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