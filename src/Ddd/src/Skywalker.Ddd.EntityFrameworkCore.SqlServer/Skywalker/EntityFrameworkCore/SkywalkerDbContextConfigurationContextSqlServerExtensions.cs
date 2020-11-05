using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.Infrastructure.DbContextConfiguration;
using Skywalker.EntityFrameworkCore.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.EntityFrameworkCore
{
    public static class SkywalkerDbContextConfigurationContextSqlServerExtensions
    {
        public static DbContextOptionsBuilder UseSqlServer(
           [NotNull] this SkywalkerDbContextConfigurationContext context,
           [MaybeNull] Action<SqlServerDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            if (context is SkywalkerEntityFrameworkCoreDbContextConfigurationContext skywalkerEntityFrameworkCoreDbContext)
            {
                if (skywalkerEntityFrameworkCoreDbContext.ExistingConnection != null)
                {
                    return skywalkerEntityFrameworkCoreDbContext.DbContextOptions.UseSqlServer(skywalkerEntityFrameworkCoreDbContext.ExistingConnection, mySQLOptionsAction);
                }
                else
                {
                    return skywalkerEntityFrameworkCoreDbContext.DbContextOptions.UseSqlServer(context.ConnectionString, mySQLOptionsAction);
                }
            }
            throw new ArgumentException("Context must is SkywalkerEntityFrameworkCoreDbContextConfigurationContext", nameof(context));
        }
    }
}
