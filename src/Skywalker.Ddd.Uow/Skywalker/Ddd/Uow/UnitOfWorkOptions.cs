// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Data;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// 
/// </summary>
public class UnitOfWorkOptions : IUnitOfWorkOptions
{
    /// <summary>
    /// Default: false.
    /// </summary>
    public bool IsTransactional { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public IsolationLevel? IsolationLevel { get; set; }

    /// <summary>
    /// Milliseconds
    /// </summary>
    public int? Timeout { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public UnitOfWorkOptions()
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="isTransactional"></param>
    /// <param name="isolationLevel"></param>
    /// <param name="timeout"></param>
    public UnitOfWorkOptions(bool isTransactional = false, IsolationLevel? isolationLevel = null, int? timeout = null)
    {
        IsTransactional = isTransactional;
        IsolationLevel = isolationLevel;
        Timeout = timeout;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
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
