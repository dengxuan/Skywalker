// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using AutoMapper;
using Skywalker.Ddd.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Skywalker.Ddd.Application;

/// <summary>
/// 应用服务基类，自动启用 UnitOfWork 拦截。
/// 默认为 Scoped 生命周期。
/// </summary>
public abstract class ApplicationService : IApplicationService, IScopedDependency
{
    /// <summary>
    /// 
    /// </summary>
    protected IMapper Mapper { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mapper"></param>
    protected ApplicationService (IMapper mapper) => Mapper = mapper;
}
