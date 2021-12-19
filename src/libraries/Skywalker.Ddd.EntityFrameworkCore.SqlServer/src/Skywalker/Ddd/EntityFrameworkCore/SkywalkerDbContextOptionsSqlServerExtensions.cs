using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextOptionsSqlServerExtensions
{
    public static void UseSqlServer(this SkywalkerDbContextOptions options, Action<SqlServerDbContextOptionsBuilder>? mySQLOptionsAction = null)
    {
        options.Configure(context =>
        {
            context.UseSqlServer(mySQLOptionsAction);
        });
    }

    public static void UseSqlServer<TDbContext>(this SkywalkerDbContextOptions options, Action<SqlServerDbContextOptionsBuilder>? mySQLOptionsAction = null)
        where TDbContext : SkywalkerDbContext<TDbContext>
    {
        //options.Services.AddMemoryCache();
        ////options.Services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

        //options.Services.TryAddTransient(typeof(ISkywalkerDbContextProvider<>), typeof(SkywalkerDbContextProvider<>));
        //options.Services.AddDbContext<TDbContext>();
        options.Configure<TDbContext>(context =>
        {
            context.UseSqlServer(mySQLOptionsAction);
        });
    }
}
