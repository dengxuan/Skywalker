using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;

namespace Microsoft.EntityFrameworkCore;

public static class DatabaseFacadeExtensions
{
#if NETSTANDARD2_0
    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
    public static bool IsRelationalDatabase(this DatabaseFacade database)
    {
        return ((IDatabaseFacadeDependenciesAccessor)database).Dependencies is IRelationalDatabaseFacadeDependencies;
    }
#else
    public static bool IsRelationalDatabase(this DatabaseFacade database)
    {
        return database.IsRelational();
    }
#endif
}
