using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
#if NET7_0
using MySql.EntityFrameworkCore.Infrastructure;
#endif
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextConfigurationContextMySqlExtensions
{
#if NETSTANDARD2_0
    public static DbContextOptionsBuilder UseMySql(this SkywalkerDbContextConfigurationContext context, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null)
    {
        if (context.ExistingConnection != null)
        {
            return context.DbContextOptions.UseMySql(context.ExistingConnection, mySQLOptionsAction);
        }
        else
        {
            return context.DbContextOptions.UseMySql(context.ConnectionString, mySQLOptionsAction);
        }
    }

#elif NETSTANDARD2_1 || NETCOREAPP3_1 || NET5_0 || NET6_0
    public static DbContextOptionsBuilder UseMySql(this SkywalkerDbContextConfigurationContext context, Action<MySqlDbContextOptionsBuilder>? mySQLOptionsAction = null)
    {
        if (context.ExistingConnection != null)
        {
            var serverVersion = ServerVersion.AutoDetect(context.ExistingConnection.ConnectionString);
            return context.DbContextOptions.UseMySql(context.ExistingConnection, serverVersion, mySQLOptionsAction);
        }
        else
        {
            var serverVersion = ServerVersion.AutoDetect(context.ConnectionString);
            return context.DbContextOptions.UseMySql(context.ConnectionString, serverVersion, mySQLOptionsAction);
        }
    }
#elif NET7_0
    public static DbContextOptionsBuilder UseMySql(this SkywalkerDbContextConfigurationContext context, Action<MySQLDbContextOptionsBuilder>? mySQLOptionsAction = null)
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
#endif
}
