// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// <see cref="IInterceptorChainBuilder"/> 的默认实现。
/// </summary>
public sealed class InterceptorChainBuilder : IInterceptorChainBuilder
{
    private readonly List<Func<InterceptorDelegate, InterceptorDelegate>> _middlewares = new();

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 初始化 <see cref="InterceptorChainBuilder"/> 类的新实例。
    /// </summary>
    /// <param name="serviceProvider">服务提供者。</param>
    public InterceptorChainBuilder(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public IInterceptorChainBuilder Use(Func<InterceptorDelegate, InterceptorDelegate> middleware)
    {
        _middlewares.Add(middleware ?? throw new ArgumentNullException(nameof(middleware)));
        return this;
    }

    /// <inheritdoc/>
    public InterceptorDelegate Build(InterceptorDelegate target)
    {
        var app = target;

        // 反向遍历中间件列表，构建管道
        // 后添加的中间件在内层，先添加的在外层
        for (var i = _middlewares.Count - 1; i >= 0; i--)
        {
            app = _middlewares[i](app);
        }

        return app;
    }
}
