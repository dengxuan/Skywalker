using System;
using System.Data.Common;
using System.Threading;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration
{
    public class SkywalkerDbContextCreationContext
    {
        public static SkywalkerDbContextCreationContext Current => _current.Value!;

        private static readonly AsyncLocal<SkywalkerDbContextCreationContext> _current = new();

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
            _current.Value = context;
            return new DisposeAction(() => _current.Value = previousValue);
        }
    }
}