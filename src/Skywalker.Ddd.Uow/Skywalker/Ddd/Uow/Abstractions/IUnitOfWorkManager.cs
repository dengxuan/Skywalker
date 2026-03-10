// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow;

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IUnitOfWorkManager
{
    /// <summary>
    /// 
    /// </summary>
    IUnitOfWork? Current { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="requiresNew"></param>
    /// <returns></returns>
    IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    /// <param name="requiresNew"></param>
    /// <returns></returns>
    IUnitOfWork Reserve(string reservationName, bool requiresNew = false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    /// <param name="options"></param>
    void BeginReserved(string reservationName, UnitOfWorkOptions options);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    bool TryBeginReserved(string reservationName, UnitOfWorkOptions options);
}
