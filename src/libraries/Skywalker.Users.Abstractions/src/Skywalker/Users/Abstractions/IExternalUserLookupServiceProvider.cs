// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Users.Abstractions;

/// <summary>
/// 用户数据查询服务提供者
/// </summary>
public interface IExternalUserLookupServiceProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IUserData?> FindByIdAsync(long id, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IUserData?> FindByUserNameAsync(string userName, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sorting"></param>
    /// <param name="filter"></param>
    /// <param name="maxResultCount"></param>
    /// <param name="skipCount"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<List<IUserData>> SearchAsync(string? sorting = null, string? filter = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> GetCountAsync(string? filter = null, CancellationToken cancellationToken = default);
}
