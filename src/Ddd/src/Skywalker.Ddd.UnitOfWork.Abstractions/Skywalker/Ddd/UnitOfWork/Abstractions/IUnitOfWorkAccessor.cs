using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface IUnitOfWorkAccessor
    {
        [MaybeNull]
        IUnitOfWork? UnitOfWork { get; }

        void SetUnitOfWork([MaybeNull] IUnitOfWork? unitOfWork);
    }
}