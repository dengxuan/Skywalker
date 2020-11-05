using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MySql.Data.EntityFrameworkCore.Infrastructure;
using Skywalker.EntityFrameworkCore.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.EntityFrameworkCore
{
    public static class SkywalkerDbContextConfigurationContextMySqlExtensions
    {
        public static DbContextOptionsBuilder UseMySql(
           [NotNull] this SkywalkerDbContextConfigurationContext context,
           [MaybeNull] Action<MySQLDbContextOptionsBuilder> mySQLOptionsAction = null)
        {
            if (context.ExistingConnection != null)
            {
                return context.DbContextOptions.UseMySQL(context.ExistingConnection, mySQLOptionsAction);
            }
            else
            {
                return context.DbContextOptions.UseMySQL(context.ConnectionString, mySQLOptionsAction);
            }
        }
    }
}
