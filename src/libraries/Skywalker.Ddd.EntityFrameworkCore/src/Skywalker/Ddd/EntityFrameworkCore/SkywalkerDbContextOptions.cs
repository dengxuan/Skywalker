using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public class SkywalkerDbContextOptions
{
    public IServiceCollection Services { get; }

    internal Action<SkywalkerDbContextConfigurationContext>? DefaultConfigureAction { get; set; }

    internal Dictionary<Type, object> ConfigureActions { get; set; }

    public SkywalkerDbContextOptions(IServiceCollection services)
    {
        Services = services;
        ConfigureActions = new Dictionary<Type, object>();
    }

    public void Configure(Action<SkywalkerDbContextConfigurationContext> action)
    {
        Check.NotNull(action, nameof(action));

        DefaultConfigureAction = action;
    }

    public void Configure<TDbContext>(Action<SkywalkerDbContextConfigurationContext<TDbContext>> action) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        Check.NotNull(action, nameof(action));

        ConfigureActions[typeof(TDbContext)] = action;
    }
}
