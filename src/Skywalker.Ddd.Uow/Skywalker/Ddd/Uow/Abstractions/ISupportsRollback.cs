// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 
/// </summary>
public interface ISupportsRollback
{
    /// <summary>
    /// 
    /// </summary>
    void Rollback();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RollbackAsync(CancellationToken cancellationToken);
}
