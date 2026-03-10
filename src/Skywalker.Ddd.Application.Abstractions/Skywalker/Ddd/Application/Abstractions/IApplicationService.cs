// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Ddd.Application.Abstractions;

/// <summary>
/// 应用服务接口，实现此接口的类将自动生成代理。
/// </summary>
public interface IApplicationService : IInterceptable
{
}
