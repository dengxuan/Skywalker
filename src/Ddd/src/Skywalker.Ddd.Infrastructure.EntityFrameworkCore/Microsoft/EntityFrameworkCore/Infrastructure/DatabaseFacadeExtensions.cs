using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Infrastructure
{
    public static class DatabaseFacadeExtensions
    {
        public static bool IsRelational(this DatabaseFacade database)
        {
            return ((IDatabaseFacadeDependenciesAccessor)database).Dependencies is IRelationalDatabaseFacadeDependencies;
        }
    }
}
