// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Uow.Abstractions;

public interface ITransactionApiContainer
{
    ITransactionApi? FindTransactionApi(string key);

    void AddTransactionApi(string key, ITransactionApi api);

    ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory);
}
