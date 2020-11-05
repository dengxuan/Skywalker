using Microsoft.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.EntityFrameworkCore
{
    public static class SkywalkerDbContextOptionsSqlServerExtensions
    {
        public static void UseSqlServer(
                [NotNull] this SkywalkerDbContextOptions options,
                [MaybeNull] Action<SqlServerDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            options.Configure(context =>
            {
                context.UseSqlServer(mySQLOptionsAction);
            });
        }

        public static void UseSqlServer<TDbContext>(
            [NotNull] this SkywalkerDbContextOptions options,
            [MaybeNull] Action<SqlServerDbContextOptionsBuilder> mySQLOptionsAction = null)
            where TDbContext : EntityFrameworkCoreDbContext<TDbContext>
        {
            options.Configure<TDbContext>(context =>
            {
                context.UseSqlServer(mySQLOptionsAction);
            });
        }
    }
}
