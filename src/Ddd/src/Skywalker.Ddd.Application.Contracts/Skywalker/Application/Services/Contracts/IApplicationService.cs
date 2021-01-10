using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.UnitOfWork.Abstractions;

namespace Skywalker.Application.Services.Contracts
{
    /// <summary>
    /// This interface must be implemented by all application services to register and identify them by convention.
    /// </summary>
    public interface IApplicationService : IUnitOfWorkEnabled, ITransientDependency
    {

    }
}
