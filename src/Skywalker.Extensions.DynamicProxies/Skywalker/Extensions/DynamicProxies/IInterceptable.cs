// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 标记接口，表示实现此接口的服务需要启用拦截代理。
/// </summary>
/// <remarks>
/// <para>
/// 实现此接口的服务在 DI 注册时会被 Castle.DynamicProxy 自动包装为代理实例，
/// 代理会注入所有注册的 <see cref="IInterceptor"/> 实现。
/// </para>
/// <para>
/// 每个拦截器通过检查目标类/方法上的特性（如 <c>[UnitOfWork]</c>、<c>[Audited]</c>）
/// 来决定是否执行拦截逻辑。
/// </para>
/// <para>
/// 使用示例：
/// <code>
/// // 定义服务接口
/// public interface IOrderService : IInterceptable
/// {
///     Task&lt;Order&gt; CreateAsync(OrderDto dto);
/// }
/// 
/// // 实现服务，标记业务特性
/// [UnitOfWork]
/// public class OrderService : IOrderService, IScopedDependency
/// {
///     public async Task&lt;Order&gt; CreateAsync(OrderDto dto) { ... }
/// }
/// </code>
/// </para>
/// </remarks>
public interface IInterceptable
{
}
