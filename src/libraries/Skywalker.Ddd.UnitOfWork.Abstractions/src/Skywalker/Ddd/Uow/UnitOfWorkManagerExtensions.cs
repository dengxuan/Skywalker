using Skywalker.Ddd.Uow.Abstractions;
using System.Data;

namespace Skywalker.Ddd.Uow;

public static class UnitOfWorkManagerExtensions
{
    public static IUnitOfWork Begin(this IUnitOfWorkManager unitOfWorkManager, bool requiresNew = false, bool isTransactional = false, IsolationLevel? isolationLevel = null, int? timeout = null)
    {
        unitOfWorkManager.NotNull(nameof(unitOfWorkManager));

        return unitOfWorkManager.Begin(new AbpUnitOfWorkOptions
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

        unitOfWorkManager.BeginReserved(reservationName, new AbpUnitOfWorkOptions());
    }

    public static void TryBeginReserved(this IUnitOfWorkManager unitOfWorkManager, string reservationName)
    {
        unitOfWorkManager.NotNull(nameof(unitOfWorkManager));
        reservationName.NotNull(nameof(reservationName));

        unitOfWorkManager.TryBeginReserved(reservationName, new AbpUnitOfWorkOptions());
    }
}
