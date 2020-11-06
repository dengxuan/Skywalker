using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Skywalker.Ddd.Infrastructure;
using Skywalker.Ddd.Infrastructure.Abstractions;
using Skywalker.Ddd.Infrastruture.Extensions.EntityFrameworkCore;
using Skywalker.EntityFrameworkCore.DependencyInjection;
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
            where TDbContext : SkywalkerDbContext<TDbContext>
        {

            options.Services.AddMemoryCache();
            //options.Services.TryAddTransient(DbContextOptionsFactory.Create<TDbContext>);

            options.Services.TryAddTransient(typeof(ISkywalkerDbContextProvider<>), typeof(SkywalkerDbContextProvider<>));
            options.Services.AddDbContext<TDbContext>();
            options.Configure<TDbContext>(context =>
            {
                context.UseSqlServer(mySQLOptionsAction);
            });
        }
    }
}
