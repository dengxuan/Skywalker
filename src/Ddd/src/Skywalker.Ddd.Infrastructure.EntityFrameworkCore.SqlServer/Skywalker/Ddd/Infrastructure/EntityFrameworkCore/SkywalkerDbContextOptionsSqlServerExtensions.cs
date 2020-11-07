using Microsoft.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore.DbContextConfiguration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore
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
}
