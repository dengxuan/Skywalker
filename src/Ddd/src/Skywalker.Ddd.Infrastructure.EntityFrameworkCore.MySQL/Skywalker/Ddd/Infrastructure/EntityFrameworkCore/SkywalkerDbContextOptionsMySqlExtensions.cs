using MySql.Data.EntityFrameworkCore.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore
{
    public static class SkywalkerDbContextOptionsMySqlExtensions
    {
        public static void UseMySql(
                [NotNull] this SkywalkerDbContextOptions options,
                [MaybeNull] Action<MySQLDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            options.Configure(context =>
            {
                context.UseMySql(mySQLOptionsAction);
            });
        }

        public static void UseMySql<TDbContext>(
            [NotNull] this SkywalkerDbContextOptions options,
            [MaybeNull] Action<MySQLDbContextOptionsBuilder> mySQLOptionsAction = null)
            where TDbContext : SkywalkerDbContext<TDbContext>
        {
            options.Configure<TDbContext>(context =>
            {
                context.UseMySql(mySQLOptionsAction);
            });
        }
    }
}
