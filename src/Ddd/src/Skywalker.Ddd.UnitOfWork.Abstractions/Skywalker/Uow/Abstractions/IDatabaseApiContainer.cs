using Skywalker.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Uow.Abstractions
{
    public interface IDatabaseApiContainer : IServiceProviderAccessor
    {
        IDatabaseApi FindDatabaseApi([NotNull] string key);

        void AddDatabaseApi([NotNull] string key, [NotNull] IDatabaseApi api);

        IDatabaseApi GetOrAddDatabaseApi([NotNull] string key, [NotNull] Func<IDatabaseApi> factory);
    }
}