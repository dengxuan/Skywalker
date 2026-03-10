// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Data;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// 
/// </summary>
public static class UnitOfWorkManagerExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitOfWorkManager"></param>
    /// <param name="requiresNew"></param>
    /// <param name="isTransactional"></param>
    /// <param name="isolationLevel"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static IUnitOfWork Begin(this IUnitOfWorkManager unitOfWorkManager, bool requiresNew = false, bool isTransactional = false, IsolationLevel? isolationLevel = null, int? timeout = null)
    {
        unitOfWorkManager.NotNull(nameof(unitOfWorkManager));

        return unitOfWorkManager.Begin(new UnitOfWorkOptions
        {
            IsTransactional = isTransactional,
            IsolationLevel = isolationLevel,
            Timeout = timeout
        }, requiresNew);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitOfWorkManager"></param>
    /// <param name="reservationName"></param>
    public static void BeginReserved(this IUnitOfWorkManager unitOfWorkManager, string reservationName)
    {
        unitOfWorkManager.NotNull(nameof(unitOfWorkManager));
        reservationName.NotNull(nameof(reservationName));

        unitOfWorkManager.BeginReserved(reservationName, new UnitOfWorkOptions());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="unitOfWorkManager"></param>
    /// <param name="reservationName"></param>
    public static void TryBeginReserved(this IUnitOfWorkManager unitOfWorkManager, string reservationName)
    {
        unitOfWorkManager.NotNull(nameof(unitOfWorkManager));
        reservationName.NotNull(nameof(reservationName));

        unitOfWorkManager.TryBeginReserved(reservationName, new UnitOfWorkOptions());
    }
}
