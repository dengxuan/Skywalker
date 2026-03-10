// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 定义拦截器链构建器接口。
/// </summary>
/// <remarks>
/// 类似于 ASP.NET Core 的 IApplicationBuilder，用于构建拦截器管道。
/// <para>
/// 使用示例：
/// <code>
/// var chain = builder
///     .Use(next => async context =>
///     {
///         Console.WriteLine("Before");
///         await next(context);
///         Console.WriteLine("After");
///     })
///     .UseInterceptor&lt;LoggingInterceptor&gt;()
///     .Build();
/// </code>
/// </para>
/// </remarks>
public interface IInterceptorChainBuilder
{
    /// <summary>
    /// 获取服务提供者。
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 添加一个中间件到拦截器链。
    /// </summary>
    /// <param name="middleware">中间件委托，接收下一个委托并返回处理委托。</param>
    /// <returns>构建器实例，支持链式调用。</returns>
    /// <remarks>
    /// 中间件按添加顺序执行，后添加的中间件包裹先添加的中间件。
    /// </remarks>
    IInterceptorChainBuilder Use(Func<InterceptorDelegate, InterceptorDelegate> middleware);

    /// <summary>
    /// 构建拦截器链并返回最终的委托。
    /// </summary>
    /// <param name="target">最终要执行的目标方法委托。</param>
    /// <returns>完整的拦截器链委托。</returns>
    InterceptorDelegate Build(InterceptorDelegate target);
}
