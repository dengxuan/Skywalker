using Skywalker.Ddd.Infrastructure.Abstractions;

namespace Skywalker.Ddd.Infrastructure.DbContextConfiguration
{
    public interface ISkywalkerDbContextConfigurer
    {
        void Configure(SkywalkerDbContextConfigurationContext context);
    }

    public interface ISkywalkerDbContextConfigurer<TDbContext> where TDbContext : SkywalkerDbContext<TDbContext>, IDbContext
    {
        void Configure(SkywalkerDbContextConfigurationContext<TDbContext> context);
    }
}