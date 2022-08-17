using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextOptionsMySqlExtensions
{
    public static SkywalkerDbContextOptions UseDbContext<TDbContext>(this SkywalkerDbContextOptions options, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContext<TDbContext>(ServiceLifetime.Transient);
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
        return options;
    }


#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NET5_0_OR_GREATER

    public static SkywalkerDbContextOptions UseDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContextFactory<TDbContext>();
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
        return options;
    }

    public static SkywalkerDbContextOptions UseDbContextPool<TDbContext>(this SkywalkerDbContextOptions options, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null, int poolSize = 1024) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContextPool<TDbContext>(builder => { }, poolSize);
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
        return options;
    }

    public static SkywalkerDbContextOptions UsePooledDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null, int poolSize = 1024) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddPooledDbContextFactory<TDbContext>(builder => { }, poolSize);
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
        return options;
    }
#endif
}
