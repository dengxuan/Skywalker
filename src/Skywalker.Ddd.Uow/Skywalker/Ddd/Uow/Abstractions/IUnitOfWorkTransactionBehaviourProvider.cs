// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 
/// </summary>
public interface IUnitOfWorkTransactionBehaviourProvider
{
    /// <summary>
    /// 
    /// </summary>
    bool? IsTransactional { get; }
}
