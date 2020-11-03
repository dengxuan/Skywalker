namespace Skywalker.EntityFrameworkCore.DependencyInjection
{
    public interface ISkywalkerDbContextConfigurer
    {
        void Configure(SkywalkerDbContextConfigurationContext context);
    }

    public interface IAbpDbContextConfigurer<TDbContext>
        where TDbContext : SkywalkerDbContext<TDbContext>
    {
        void Configure(AbpDbContextConfigurationContext<TDbContext> context);
    }
}