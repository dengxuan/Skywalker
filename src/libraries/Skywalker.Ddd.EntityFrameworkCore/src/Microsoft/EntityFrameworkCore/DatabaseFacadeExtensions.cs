using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.EntityFrameworkCore;

public static class DatabaseFacadeExtensions
{
#if NETSTANDARD2_0
    [SuppressMessage("Usage", "EF1001:Internal EF Core API usage.", Justification = "<Pending>")]
    public static bool IsRelational(this DatabaseFacade database)
    {
        return ((IDatabaseFacadeDependenciesAccessor)database).Dependencies is IRelationalDatabaseFacadeDependencies;
    }
#endif
}
