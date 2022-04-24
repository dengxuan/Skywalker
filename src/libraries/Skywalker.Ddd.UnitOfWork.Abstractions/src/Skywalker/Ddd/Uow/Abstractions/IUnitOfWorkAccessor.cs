// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Uow.Abstractions;

public interface IUnitOfWorkAccessor: ISingletonDependency
{
    IUnitOfWork? UnitOfWork { get; }

    void SetUnitOfWork( IUnitOfWork? unitOfWork);
}
