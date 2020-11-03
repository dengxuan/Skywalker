using Skywalker.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public static class SkywalkerMySqlModelBuilderExtensions
    {
        public static void UseMySQL(this ModelBuilder modelBuilder)
        {
            modelBuilder.SetDatabaseProvider(EfCoreDatabaseProvider.MySql);
        }
    }
}