using Skywalker.Ddd.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class SkywalkerSqlServerModelBuilderExtensions
    {
        public static void UseSqlServer(this ModelBuilder modelBuilder)
        {
            modelBuilder.SetDatabaseProvider(EntityFrameworkCoreDatabaseProvider.SqlServer);
        }
    }
}