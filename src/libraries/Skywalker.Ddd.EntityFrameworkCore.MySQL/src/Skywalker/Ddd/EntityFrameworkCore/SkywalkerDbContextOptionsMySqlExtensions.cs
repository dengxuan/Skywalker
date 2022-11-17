using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
#if NET7_0
using MySql.EntityFrameworkCore.Infrastructure;
#endif
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextOptionsMySqlExtensions
{

#if NETSTANDARD2_0
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
#elif NETSTANDARD2_1 || NETCOREAPP3_1 || NET5_0 || NET6_0

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
#elif NET7_0

    public static SkywalkerDbContextOptions UseDbContext<TDbContext>(this SkywalkerDbContextOptions options, Action<MySQLDbContextOptionsBuilder>? mySQLOptionsAction = null) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContext<TDbContext>(ServiceLifetime.Transient);
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
        return options;
    }

    public static SkywalkerDbContextOptions UseDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<MySQLDbContextOptionsBuilder>? mySQLOptionsAction = null) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContextFactory<TDbContext>();
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
        return options;
    }

    public static SkywalkerDbContextOptions UseDbContextPool<TDbContext>(this SkywalkerDbContextOptions options, Action<MySQLDbContextOptionsBuilder>? mySQLOptionsAction = null, int poolSize = 1024) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Services.TryAddTransient(SkywalkerDbContextOptionsFactory.Create<TDbContext>);
        options.Services.AddDbContextPool<TDbContext>(builder => { }, poolSize);
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
        return options;
    }

    public static SkywalkerDbContextOptions UsePooledDbContextFactory<TDbContext>(this SkywalkerDbContextOptions options, Action<MySQLDbContextOptionsBuilder>? mySQLOptionsAction = null, int poolSize = 1024) where TDbContext : SkywalkerDbContext<TDbContext>
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
