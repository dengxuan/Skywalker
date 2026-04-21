// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using Skywalker.Ddd.Application.Abstractions;

namespace Skywalker.Ddd.Application;

/// <summary>
/// 应用服务基类，自动启用 UnitOfWork 拦截。
/// 默认为 Scoped 生命周期。
/// </summary>
/// <remarks>
/// 框架不再内置对象映射器。如需对象映射，推荐在业务项目中引用
/// <see href="https://github.com/riok/mapperly">Riok.Mapperly</see> 源生成器，
/// 通过 <c>[Mapper] partial class</c> 声明映射，编译期生成实现，零运行时开销且 AOT 友好。
/// </remarks>
public abstract class ApplicationService : IApplicationService
{
}
