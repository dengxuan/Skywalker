// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Uow.Abstractions;

public interface IUnitOfWorkManager//: ISingletonDependency
{
    IUnitOfWork? Current { get; }

    IUnitOfWork Begin(UnitOfWorkOptions options, bool requiresNew = false);

    IUnitOfWork Reserve(string reservationName, bool requiresNew = false);

    void BeginReserved(string reservationName, UnitOfWorkOptions options);

    bool TryBeginReserved(string reservationName, UnitOfWorkOptions options);
}
