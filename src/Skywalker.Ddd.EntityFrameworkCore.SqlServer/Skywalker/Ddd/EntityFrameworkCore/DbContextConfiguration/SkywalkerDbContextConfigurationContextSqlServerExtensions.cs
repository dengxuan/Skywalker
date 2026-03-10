using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

public static class SkywalkerDbContextConfigurationContextSqlServerExtensions
{
#if NET7_0
    public static DbContextOptionsBuilder UseSqlServer(this SkywalkerDbContextConfigurationContext context, Action<SqlServerDbContextOptionsBuilder>? sqlserverOptionsAction = null)
    {
        if (context.ExistingConnection != null)
        {
            return context.DbContextOptions.UseSqlServer(context.ExistingConnection, sqlserverOptionsAction);
        }
        else
        {
            return context.DbContextOptions.UseSqlServer(context.ConnectionString, sqlserverOptionsAction);
        }
    }
#else
    public static DbContextOptionsBuilder UseSqlServer(this SkywalkerDbContextConfigurationContext context, Action<SqlServerDbContextOptionsBuilder>? sqlserverOptionsAction = null)
    {
        if (context.ExistingConnection != null)
        {
            return context.DbContextOptions.UseSqlServer(context.ExistingConnection, sqlserverOptionsAction);
        }
        else
        {
            return context.DbContextOptions.UseSqlServer(context.ConnectionString, sqlserverOptionsAction);
        }
    }
#endif
}
