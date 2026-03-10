// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Uow.Abstractions;
using Skywalker.DependencyInjection;

namespace Skywalker.Ddd.Uow;

/// <summary>
/// 空实现的工作单元事务行为提供者
/// </summary>
public class NullUnitOfWorkTransactionBehaviourProvider : IUnitOfWorkTransactionBehaviourProvider, ISingletonDependency
{
    /// <summary>
    /// 
    /// </summary>
    public bool? IsTransactional => null;
}
