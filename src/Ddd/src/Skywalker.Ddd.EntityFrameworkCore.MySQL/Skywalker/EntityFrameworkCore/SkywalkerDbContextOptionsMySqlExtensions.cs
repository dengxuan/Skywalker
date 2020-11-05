using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.EntityFrameworkCore
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
            where TDbContext : EntityFrameworkCoreDbContext<TDbContext>,IDbContext
        {
            options.Configure<TDbContext>(context =>
            {
                context.UseMySql(mySQLOptionsAction);
            });
        }
    }
}
