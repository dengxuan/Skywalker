using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Infrastructure.DbContextConfiguration
{
    public class SkywalkerDbContextConfigurationContext : IServiceProviderAccessor
    {
        public IServiceProvider ServiceProvider { get; }

        public string ConnectionString { get; }

        public string ConnectionStringName { get; }

        public SkywalkerDbContextConfigurationContext([NotNull] string connectionString, [NotNull] IServiceProvider serviceProvider, [MaybeNull] string connectionStringName)
        {
            ConnectionString = connectionString;
            ServiceProvider = serviceProvider;
            ConnectionStringName = connectionStringName;
        }
    }

    public class SkywalkerDbContextConfigurationContext<TDbContext> : SkywalkerDbContextConfigurationContext where TDbContext : SkywalkerDbContext<TDbContext>
    {
        public SkywalkerDbContextConfigurationContext(string connectionString, [NotNull] IServiceProvider serviceProvider, [MaybeNull] string connectionStringName, [MaybeNull] DbConnection existingConnection) : base(connectionString, serviceProvider, connectionStringName)
        {
        }
    }
}