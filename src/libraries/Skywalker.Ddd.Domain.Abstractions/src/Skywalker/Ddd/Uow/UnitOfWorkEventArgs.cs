// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public class UnitOfWorkEventArgs : EventArgs
{
    /// <summary>
    /// Reference to the unit of work related to this event.
    /// </summary>
    public IUnitOfWork UnitOfWork { get; }

    public UnitOfWorkEventArgs(IUnitOfWork unitOfWork)
    {
        unitOfWork.NotNull(nameof(unitOfWork));

        UnitOfWork = unitOfWork;
    }
}
