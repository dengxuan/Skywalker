using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Uow.Abstractions
{
    public interface IUnitOfWorkAccessor
    {
        [MaybeNull]
        IUnitOfWork UnitOfWork { get; }

        void SetUnitOfWork([MaybeNull] IUnitOfWork unitOfWork);
    }
}