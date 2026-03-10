// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Threading.Tasks;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 表示可以处理方法调用的函数委托。
/// </summary>
/// <param name="context">方法调用上下文。</param>
/// <returns>表示异步操作的任务。</returns>
/// <remarks>
/// 类似于 ASP.NET Core 的 RequestDelegate，用于构建拦截器管道。
/// </remarks>
public delegate Task InterceptorDelegate(IMethodInvocation context);
