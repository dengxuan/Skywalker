using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Infrastructure.EntityFrameworkCore.DbContextConfiguration
{
    public static class SkywalkerDbContextConfigurationContextSqlServerExtensions
    {
        public static DbContextOptionsBuilder UseSqlServer(
           [NotNull] this SkywalkerDbContextConfigurationContext context,
           [MaybeNull] Action<SqlServerDbContextOptionsBuilder>? mySQLOptionsAction = null)
        {
                if (context.ExistingConnection != null)
                {
                    return context.DbContextOptions.UseSqlServer(context.ExistingConnection, mySQLOptionsAction);
                }
                else
                {
                    return context.DbContextOptions.UseSqlServer(context.ConnectionString, mySQLOptionsAction);
                }
        }
    }
}
