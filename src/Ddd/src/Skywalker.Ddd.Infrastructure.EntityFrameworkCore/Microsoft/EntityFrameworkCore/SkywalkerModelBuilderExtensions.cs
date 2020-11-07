using Skywalker.Ddd.Infrastructure.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class SkywalkerModelBuilderExtensions
    {
        private const string ModelDatabaseProviderAnnotationKey = "_Skywalker_DatabaseProvider";

        public static void SetDatabaseProvider(
            this ModelBuilder modelBuilder,
            EntityFrameworkCoreDatabaseProvider databaseProvider)
        {
            modelBuilder.Model.SetAnnotation(ModelDatabaseProviderAnnotationKey, databaseProvider);
        }

        public static void ClearDatabaseProvider(
            this ModelBuilder modelBuilder)
        {
            modelBuilder.Model.RemoveAnnotation(ModelDatabaseProviderAnnotationKey);
        }

        public static EntityFrameworkCoreDatabaseProvider? GetDatabaseProvider(
            this ModelBuilder modelBuilder
        )
        {
            return (EntityFrameworkCoreDatabaseProvider?) modelBuilder.Model[ModelDatabaseProviderAnnotationKey];
        }

        public static bool IsUsingMySQL(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.MySql;
        }

        public static bool IsUsingOracle(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.Oracle;
        }

        public static bool IsUsingSqlServer(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.SqlServer;
        }

        public static bool IsUsingPostgreSql(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.PostgreSql;
        }

        public static bool IsUsingSqlite(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.Sqlite;
        }

        public static void UseInMemory(
            this ModelBuilder modelBuilder)
        {
            modelBuilder.SetDatabaseProvider(EntityFrameworkCoreDatabaseProvider.InMemory);
        }
        
        public static bool IsUsingInMemory(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.InMemory;
        }

        public static void UseCosmos(
            this ModelBuilder modelBuilder)
        {
            modelBuilder.SetDatabaseProvider(EntityFrameworkCoreDatabaseProvider.Cosmos);
        }
        
        public static bool IsUsingCosmos(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.Cosmos;
        }

        public static void UseFirebird(
            this ModelBuilder modelBuilder)
        {
            modelBuilder.SetDatabaseProvider(EntityFrameworkCoreDatabaseProvider.Firebird);
        }
        
        public static bool IsUsingFirebird(
            this ModelBuilder modelBuilder)
        {
            return modelBuilder.GetDatabaseProvider() == EntityFrameworkCoreDatabaseProvider.Firebird;
        }
    }
}