// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Abstractions;

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IDatabaseApiContainer : IServiceProviderAccessor
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    IDatabaseApi? FindDatabaseApi(string key);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="api"></param>
    void AddDatabaseApi(string key, IDatabaseApi api);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    IDatabaseApi GetOrAddDatabaseApi(string key, Func<IDatabaseApi> factory);
}
