using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Uow.Abstractions
{
    public interface IUnitOfWorkManager
    {
        [MaybeNull]
        IUnitOfWork Current { get; }

        IUnitOfWork Begin([NotNull] AbpUnitOfWorkOptions options, bool requiresNew = false);

        IUnitOfWork Reserve([NotNull] string reservationName, bool requiresNew = false);

        void BeginReserved([NotNull] string reservationName, [NotNull] AbpUnitOfWorkOptions options);

        bool TryBeginReserved([NotNull] string reservationName, [NotNull] AbpUnitOfWorkOptions options);
    }
}