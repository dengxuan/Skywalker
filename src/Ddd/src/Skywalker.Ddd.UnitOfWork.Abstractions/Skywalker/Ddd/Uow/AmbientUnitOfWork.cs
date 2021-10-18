using Microsoft.Extensions.DependencyInjection;
using Skywalker.Ddd.Uow.Abstractions;
using System.Diagnostics.CodeAnalysis;

namespace Skywalker.Ddd.Uow;

public class AmbientUnitOfWork : IAmbientUnitOfWork, ISingletonDependency
{
    [MaybeNull]
    public IUnitOfWork? UnitOfWork => _currentUow.Value;

    private readonly AsyncLocal<IUnitOfWork?> _currentUow;

    public AmbientUnitOfWork()
    {
        _currentUow = new AsyncLocal<IUnitOfWork?>();
    }

    public void SetUnitOfWork([MaybeNull] IUnitOfWork? unitOfWork)
    {
        _currentUow.Value = unitOfWork;
    }
}
