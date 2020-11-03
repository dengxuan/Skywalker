using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.EntityFrameworkCore
{
    public static class SkywalkerDbContextOptionsMySqlExtensions
    {
        public static void UseSqlServer(
                [NotNull] this SkywalkerDbContextOptions options,
                [MaybeNull] Action<MySqlDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            options.Configure(context =>
            {
                context.UseMySql(mySQLOptionsAction);
            });
        }

        public static void UseSqlServer<TDbContext>(
            [NotNull] this SkywalkerDbContextOptions options,
            [MaybeNull] Action<MySqlDbContextOptionsBuilder> mySQLOptionsAction = null)
            where TDbContext : SkywalkerDbContext<TDbContext>
        {
            options.Configure<TDbContext>(context =>
            {
                context.UseMySql(mySQLOptionsAction);
            });
        }
    }
}
