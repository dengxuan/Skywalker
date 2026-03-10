using System.Data.Common;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration;

public class SkywalkerDbContextCreationContext
{
    public static SkywalkerDbContextCreationContext Current => s_current.Value!;

    private static readonly AsyncLocal<SkywalkerDbContextCreationContext> s_current = new();

    public string ConnectionStringName { get; }

    public string ConnectionString { get; }

    public DbConnection? ExistingConnection { get; set; }

    public SkywalkerDbContextCreationContext(string connectionStringName, string connectionString)
    {
        ConnectionStringName = connectionStringName;
        ConnectionString = connectionString;
    }

    public static IDisposable Use(SkywalkerDbContextCreationContext context)
    {
        var previousValue = Current;
        s_current.Value = context;
        return new DisposeAction(() => s_current.Value = previousValue);
    }
}
