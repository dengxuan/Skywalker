using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.UnitOfWork.Abstractions;

public interface IDatabaseApiContainer : IServiceProviderAccessor
{
    IDatabaseApi? FindDatabaseApi(string key);

    void AddDatabaseApi(string key, IDatabaseApi api);

    IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory);
}
