// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.


// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 
/// </summary>
public interface ITransactionApiContainer
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    ITransactionApi? FindTransactionApi(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="api"></param>
    void AddTransactionApi(string key, ITransactionApi api);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    ITransactionApi GetOrAddTransactionApi(string key, Func<ITransactionApi> factory);
}
