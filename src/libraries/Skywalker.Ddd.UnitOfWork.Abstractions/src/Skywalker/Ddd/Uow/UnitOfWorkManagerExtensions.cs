// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Data;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public static class UnitOfWorkManagerExtensions
{
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

    public static void BeginReserved(this IUnitOfWorkManager unitOfWorkManager, string reservationName)
    {
        unitOfWorkManager.NotNull(nameof(unitOfWorkManager));
        reservationName.NotNull(nameof(reservationName));

        unitOfWorkManager.BeginReserved(reservationName, new UnitOfWorkOptions());
    }

    public static void TryBeginReserved(this IUnitOfWorkManager unitOfWorkManager, string reservationName)
    {
        unitOfWorkManager.NotNull(nameof(unitOfWorkManager));
        reservationName.NotNull(nameof(reservationName));

        unitOfWorkManager.TryBeginReserved(reservationName, new UnitOfWorkOptions());
    }
}
