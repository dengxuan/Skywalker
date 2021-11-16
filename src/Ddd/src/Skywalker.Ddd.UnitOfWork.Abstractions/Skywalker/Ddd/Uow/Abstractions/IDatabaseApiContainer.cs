﻿using Skywalker.Ddd.DependencyInjection;

namespace Skywalker.Ddd.Uow.Abstractions;

public interface IDatabaseApiContainer : IServiceProviderAccessor
{
    IDatabaseApi? FindDatabaseApi(string key);

    void AddDatabaseApi(string key, IDatabaseApi api);

    IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory);
}
