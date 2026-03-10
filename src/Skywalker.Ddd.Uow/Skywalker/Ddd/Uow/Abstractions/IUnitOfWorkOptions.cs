// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Data;

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IUnitOfWorkOptions
{
    /// <summary>
    /// 
    /// </summary>
    bool IsTransactional { get; }

    /// <summary>
    /// 
    /// </summary>
    IsolationLevel? IsolationLevel { get; }

    /// <summary>
    /// Milliseconds
    /// </summary>
    int? Timeout { get; }
}
