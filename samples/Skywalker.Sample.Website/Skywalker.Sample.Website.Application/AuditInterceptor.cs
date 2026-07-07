using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Skywalker.Extensions.DynamicProxies;

namespace Skywalker.Sample.Website.Application;

/// <summary>
/// 审计拦截器：记录被代理服务的每次方法调用（smoke 验证用 Invocations 计数断言拦截生效）。
/// </summary>
public sealed class AuditInterceptor(ILogger<AuditInterceptor> logger) : IInterceptor
{
    public static ConcurrentQueue<string> Invocations { get; } = new();

    public async Task InterceptAsync(IMethodInvocation invocation)
    {
        Invocations.Enqueue(invocation.MethodName);
        logger.LogInformation("[Audit] {Method} invoked", invocation.MethodName);
        await invocation.ProceedAsync();
    }
}
