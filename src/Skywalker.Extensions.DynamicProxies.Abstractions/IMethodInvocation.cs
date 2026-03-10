using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Skywalker.Extensions.DynamicProxies;

/// <summary>
/// 表示一个方法调用的上下文。
/// </summary>
/// <remarks>
/// 此接口用于拦截器中获取和修改方法调用的信息。
/// </remarks>
public interface IMethodInvocation
{
    /// <summary>
    /// 获取被调用的目标对象。
    /// </summary>
    object Target { get; }

    /// <summary>
    /// 获取被调用的方法信息。
    /// </summary>
    MethodInfo Method { get; }

    /// <summary>
    /// 获取方法名称。
    /// </summary>
    string MethodName { get; }

    /// <summary>
    /// 获取方法参数。
    /// </summary>
    object?[] Arguments { get; }

    /// <summary>
    /// 获取方法返回类型。
    /// </summary>
    Type ReturnType { get; }

    /// <summary>
    /// 获取或设置方法返回值。
    /// </summary>
    /// <remarks>
    /// 对于 void 方法，此值为 null。
    /// 对于异步方法，此值为 Task 或 Task&lt;T&gt; 的结果。
    /// </remarks>
    object? ReturnValue { get; set; }

    /// <summary>
    /// 继续执行方法调用链（调用下一个拦截器或实际方法）。
    /// </summary>
    /// <returns>表示异步操作的任务。</returns>
    Task ProceedAsync();
}
