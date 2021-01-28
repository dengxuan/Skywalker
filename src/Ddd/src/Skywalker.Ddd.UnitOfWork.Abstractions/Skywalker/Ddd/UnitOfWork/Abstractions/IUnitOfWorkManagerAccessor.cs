using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.UnitOfWork.Abstractions
{
    public interface IUnitOfWorkManagerAccessor: IScopedDependency
    {
        IUnitOfWorkManager? UnitOfWorkManager { get; }
    }
}
