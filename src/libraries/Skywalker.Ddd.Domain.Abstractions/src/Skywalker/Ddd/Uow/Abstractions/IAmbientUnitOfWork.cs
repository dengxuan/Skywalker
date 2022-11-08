// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Uow.Abstractions;

/// <summary>
/// 当前线程环境下的工作单元
/// </summary>
public interface IAmbientUnitOfWork : ISingletonDependency, IUnitOfWorkAccessor
{

}
