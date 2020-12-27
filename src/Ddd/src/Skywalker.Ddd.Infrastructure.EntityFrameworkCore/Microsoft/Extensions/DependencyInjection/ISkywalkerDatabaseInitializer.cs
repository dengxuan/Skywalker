using System;
using System.Collections.Generic;

namespace Microsoft.Extensions.DependencyInjection
{
    internal interface ISkywalkerDatabaseInitializer
    {
        void AddDatabases(IEnumerable<Type> entityTypes);
    }
}
