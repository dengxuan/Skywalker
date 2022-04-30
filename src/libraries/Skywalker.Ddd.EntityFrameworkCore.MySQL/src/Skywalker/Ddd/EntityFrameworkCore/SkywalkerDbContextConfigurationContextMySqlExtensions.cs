using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextConfigurationContextMySqlExtensions
{
#if NETSTANDARD2_0
    public static DbContextOptionsBuilder UseMySql(this SkywalkerDbContextConfigurationContext context, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null)
    {
        if (context.ExistingConnection != null)
        {
            return context.DbContextOptions.UseMySql(context.ExistingConnection, mySQLOptionsAction).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
        else
        {
            return context.DbContextOptions.UseMySql(context.ConnectionString, mySQLOptionsAction).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }

#elif NETSTANDARD2_1_OR_GREATER || NETCOREAPP || NET5_0_OR_GREATER
    public static DbContextOptionsBuilder UseMySql(this SkywalkerDbContextConfigurationContext context, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null)
    {
        if (context.ExistingConnection != null)
        {
            var serverVersion = ServerVersion.AutoDetect(context.ExistingConnection.ConnectionString);
            return context.DbContextOptions.UseMySql(context.ExistingConnection, serverVersion, mySQLOptionsAction).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
        else
        {
            var serverVersion = ServerVersion.AutoDetect(context.ConnectionString);
            return context.DbContextOptions.UseMySql(context.ConnectionString, serverVersion, mySQLOptionsAction).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); ;
        }
    }
#endif
}
