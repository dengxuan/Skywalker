using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore;

public static class DatabaseFacadeExtensions
{
    public static bool IsRelationalDatabase(this DatabaseFacade database)
    {
        return database.IsRelational();
    }
}
