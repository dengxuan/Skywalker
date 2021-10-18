﻿namespace Skywalker.Ddd.Uow.Abstractions;

public interface ITransactionApiContainer
{
    ITransactionApi? FindTransactionApi(string key);

    void AddTransactionApi(string key, ITransactionApi api);

    ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory);
}
