using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextOptionsMySqlExtensions
{
    public static void UseMySql(this SkywalkerDbContextOptions options, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null)
    {
        options.Configure(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
    }

    public static void UseMySql<TDbContext>(this SkywalkerDbContextOptions options, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null) where TDbContext : SkywalkerDbContext<TDbContext>
    {
        options.Configure<TDbContext>(context =>
        {
            context.UseMySql(mySQLOptionsAction);
        });
    }
}
