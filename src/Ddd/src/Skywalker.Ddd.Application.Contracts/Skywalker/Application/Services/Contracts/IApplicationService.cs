using Microsoft.Extensions.DependencyInjection;
using Skywalker.Aspects;
using Skywalker.Uow;
using Skywalker.Uow.Abstractions;

namespace Skywalker.Application.Services.Contracts
{
    /// <summary>
    /// This interface must be implemented by all application services to register and identify them by convention.
    /// </summary>
    [Aspects]
    [UnitOfWork]
    public interface IApplicationService : IAspects, IUnitOfWorkEnabled, ITransientDependency
    {

    }
}
