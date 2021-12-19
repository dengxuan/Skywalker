// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public static class UnitOfWorkExtensions
{
    public static bool IsReservedFor(this IUnitOfWork unitOfWork, string reservationName)
    {
        unitOfWork.NotNull(nameof(unitOfWork));

        return unitOfWork.IsReserved && unitOfWork.ReservationName == reservationName;
    }
}
