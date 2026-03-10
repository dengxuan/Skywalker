using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

namespace Skywalker.Ddd.EntityFrameworkCore;

public static class SkywalkerDbContextConfigurationContextMySqlExtensions
{
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
}
