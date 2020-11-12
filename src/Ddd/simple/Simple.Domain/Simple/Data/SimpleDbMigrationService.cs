using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Skywalker.Data.Seeding;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simple.Data
{
    public class SimpleDbMigrationService : ITransientDependency
    {
        protected readonly ILogger<SimpleDbMigrationService> Logger;

        private readonly IDataSeeder _dataSeeder;
        private readonly IEnumerable<ISimpleDbSchemaMigrator> _dbSchemaMigrators;

        public SimpleDbMigrationService(IDataSeeder dataSeeder, IEnumerable<ISimpleDbSchemaMigrator> dbSchemaMigrators, ILogger<SimpleDbMigrationService> logger)
        {
            _dataSeeder = dataSeeder;
            _dbSchemaMigrators = dbSchemaMigrators;
            Logger = logger;
        }

        public async Task MigrateAsync()
        {
            Logger.LogInformation("Started database migrations...");

            await MigrateDatabaseSchemaAsync();
            await SeedDataAsync();

            Logger.LogInformation($"Successfully completed host database migrations.");
        }

        private async Task MigrateDatabaseSchemaAsync()
        {
            Logger.LogInformation($"Migrating schema database...");

            foreach (var migrator in _dbSchemaMigrators)
            {
                await migrator.MigrateAsync();
            }
        }

        private async Task SeedDataAsync()
        {
            Logger.LogInformation($"Executing  database seed...");

            await _dataSeeder.SeedAsync();
        }
    }
}
