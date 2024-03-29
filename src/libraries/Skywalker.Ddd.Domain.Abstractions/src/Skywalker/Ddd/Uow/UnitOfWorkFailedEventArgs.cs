﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// Used as event arguments on <see cref="IUnitOfWork.Failed"/> event.
/// </summary>
public class UnitOfWorkFailedEventArgs : UnitOfWorkEventArgs
{
    /// <summary>
    /// Exception that caused failure. This is set only if an error occurred during <see cref="IUnitOfWork.Complete"/>.
    /// Can be null if there is no exception, but <see cref="IUnitOfWork.Complete"/> is not called. 
    /// Can be null if another exception occurred during the UOW.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// True, if the unit of work is manually rolled back.
    /// </summary>
    public bool IsRolledback { get; }

    /// <summary>
    /// Creates a new <see cref="UnitOfWorkFailedEventArgs"/> object.
    /// </summary>
    public UnitOfWorkFailedEventArgs(IUnitOfWork unitOfWork, Exception? exception, bool isRolledback)
        : base(unitOfWork)
    {
        Exception = exception;
        IsRolledback = isRolledback;
    }
}
