using Microsoft.EntityFrameworkCore;
using MySql.Data.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.Infrastructure.EntityFrameworkCore.DbContextConfiguration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore
{
    public static class SkywalkerDbContextConfigurationContextMySqlExtensions
    {
        public static DbContextOptionsBuilder UseMySql(
           [NotNull] this SkywalkerDbContextConfigurationContext context,
           [MaybeNull] Action<MySQLDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            //if (context is SkywalkerEntityFrameworkCoreDbContextConfigurationContext skywalkerEntityFrameworkCoreDbContext)
            //{
            //    if (skywalkerEntityFrameworkCoreDbContext.ExistingConnection != null)
            //    {
            //        return skywalkerEntityFrameworkCoreDbContext.DbContextOptions.UseMySQL(skywalkerEntityFrameworkCoreDbContext.ExistingConnection, mySQLOptionsAction);
            //    }
            //    else
            //    {
            //        return skywalkerEntityFrameworkCoreDbContext.DbContextOptions.UseMySQL(context.ConnectionString, mySQLOptionsAction);
            //    }
            //}
            throw new ArgumentException("Context must is SkywalkerEntityFrameworkCoreDbContextConfigurationContext", nameof(context));
        }
    }
}
