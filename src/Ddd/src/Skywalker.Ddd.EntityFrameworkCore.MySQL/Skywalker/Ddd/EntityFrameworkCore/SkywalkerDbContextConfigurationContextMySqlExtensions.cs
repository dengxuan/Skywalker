using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore
{
    public static class SkywalkerDbContextConfigurationContextMySqlExtensions
    {
        public static DbContextOptionsBuilder UseMySql([NotNull] this SkywalkerDbContextConfigurationContext context, [MaybeNull] Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null)
        {
            if (context.ExistingConnection != null)
            {
                return context.DbContextOptions.UseMySql(context.ExistingConnection, ServerVersion.AutoDetect(context.ExistingConnection.ConnectionString), mySQLOptionsAction);
            }
            else
            {
                return context.DbContextOptions.UseMySql(context.ConnectionString, ServerVersion.AutoDetect(context.ConnectionString), mySQLOptionsAction);
            }
        }
    }
}
