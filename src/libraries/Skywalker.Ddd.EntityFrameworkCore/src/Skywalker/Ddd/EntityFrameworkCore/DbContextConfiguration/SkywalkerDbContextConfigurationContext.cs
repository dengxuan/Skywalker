using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.EntityFrameworkCore.DbContextConfiguration
{
    public class SkywalkerDbContextConfigurationContext : IServiceProviderAccessor
    {
        public IServiceProvider ServiceProvider { get; }

        public string ConnectionString { get; }

        public string ConnectionStringName { get; }

        public DbConnection? ExistingConnection { get; }

        public DbContextOptionsBuilder DbContextOptions { get; protected set; }

        public SkywalkerDbContextConfigurationContext(
            string connectionString,
            IServiceProvider serviceProvider,
             string connectionStringName,
             DbConnection? existingConnection)
        {
            ConnectionString = connectionString;
            ServiceProvider = serviceProvider;
            ConnectionStringName = connectionStringName;
            ExistingConnection = existingConnection;

            DbContextOptions = new DbContextOptionsBuilder()
                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());
        }
    }

    public class SkywalkerDbContextConfigurationContext<TDbContext> : SkywalkerDbContextConfigurationContext
        where TDbContext : SkywalkerDbContext<TDbContext>
    {
        public new DbContextOptionsBuilder<TDbContext> DbContextOptions => (DbContextOptionsBuilder<TDbContext>)base.DbContextOptions;

        public SkywalkerDbContextConfigurationContext(
            string connectionString,
            IServiceProvider serviceProvider,
             string connectionStringName,
             DbConnection? existingConnection)
            : base(
                  connectionString,
                  serviceProvider,
                  connectionStringName,
                  existingConnection)
        {
            base.DbContextOptions = new DbContextOptionsBuilder<TDbContext>()
                .UseLoggerFactory(serviceProvider.GetRequiredService<ILoggerFactory>());
        }
    }
}