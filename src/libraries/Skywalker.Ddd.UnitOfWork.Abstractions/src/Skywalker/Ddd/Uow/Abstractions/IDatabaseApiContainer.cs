// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection.Abstractions;

namespace Skywalker.Ddd.Uow.Abstractions;

public interface IDatabaseApiContainer : IServiceProviderAccessor
{
    IDatabaseApi? FindDatabaseApi(string key);

    void AddDatabaseApi(string key, IDatabaseApi api);

    IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory);
}
