using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface IUnitOfWorkAccessor : ISingletonDependency
    {
        [MaybeNull]
        IUnitOfWork? UnitOfWork { get; }

        void SetUnitOfWork([MaybeNull] IUnitOfWork? unitOfWork);
    }
}