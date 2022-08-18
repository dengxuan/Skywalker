// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Data;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public class UnitOfWorkOptions : IUnitOfWorkOptions
{
    /// <summary>
    /// Default: false.
    /// </summary>
    public bool IsTransactional { get; set; }

    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// Milliseconds
    /// </summary>
    public int? Timeout { get; set; }

    public UnitOfWorkOptions()
    {

    }

    public UnitOfWorkOptions(bool isTransactional = false, IsolationLevel? isolationLevel = null, int? timeout = null)
    {
        IsTransactional = isTransactional;
        IsolationLevel = isolationLevel;
        Timeout = timeout;
    }

    public UnitOfWorkOptions Clone()
    {
        return new UnitOfWorkOptions
        {
            IsTransactional = IsTransactional,
            IsolationLevel = IsolationLevel,
            Timeout = Timeout
        };
    }
}
