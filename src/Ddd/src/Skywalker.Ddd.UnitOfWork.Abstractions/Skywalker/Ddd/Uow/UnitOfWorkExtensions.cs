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
