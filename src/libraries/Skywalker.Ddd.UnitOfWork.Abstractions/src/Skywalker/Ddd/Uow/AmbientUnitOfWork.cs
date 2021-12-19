// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public class AmbientUnitOfWork : IAmbientUnitOfWork/*, ISingletonDependency*/
{
    
    public IUnitOfWork? UnitOfWork => _currentUow.Value;

    private readonly AsyncLocal<IUnitOfWork?> _currentUow;

    public AmbientUnitOfWork()
    {
        _currentUow = new AsyncLocal<IUnitOfWork?>();
    }

    public void SetUnitOfWork( IUnitOfWork? unitOfWork)
    {
        _currentUow.Value = unitOfWork;
    }
}
